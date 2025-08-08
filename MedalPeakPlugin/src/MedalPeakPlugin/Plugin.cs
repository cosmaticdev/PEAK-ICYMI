using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Newtonsoft.Json;
using Steamworks;
using System;
using UnityEngine.Rendering.Universal;

#nullable enable
namespace MedalPeakPlugin;

[BepInPlugin("cosmatic.MedalPeakPlugin", "MedalPeakPlugin", "1.0")]
public class MedalPeakPlugin : BaseUnityPlugin
{
    internal static MedalPeakPlugin? Instance { get; private set; }
    internal Harmony? Harmony { get; set; }
    internal new static ManualLogSource Logger { get; private set; } = null!;

    internal static float timeOfLastClip = 1;
    internal static bool fallCausedByScoutmaster = false;

    private static readonly HttpClient httpClient = new HttpClient();
    private static LobbyChatUpdateHandler? currentLobbyHandler;

    private void Awake()
    {
        Instance = this;
        Logger = base.Logger;

        httpClient.DefaultRequestHeaders.Add("publicKey", "pub_7yyLREtjlmTJeGtpI8wWR9NxpIkuvTF1");

        gameObject.transform.parent = null;
        gameObject.hideFlags = (HideFlags)61;
        Patch();
        Logger.LogInfo($"{Info.Metadata.GUID} v{Info.Metadata.Version} has loaded!");
    }

    internal void Patch()
    {
        if (Harmony == null)
        {
            Harmony = _ = new Harmony("cosmatic.MedalPeakPlugin");
        }
        Harmony.PatchAll();
    }

    internal void Unpatch() => Harmony?.UnpatchSelf();

    // get all players in a lobby when you join
    [HarmonyPatch(typeof(SteamLobbyHandler), "OnLobbyEnter")]
    public class LobbyEnterPatch
    {
        private static void Prefix(in LobbyEnter_t param)
        {
            if (param.m_EChatRoomEnterResponse == 2)
            {
                MedalPeakPlugin.Logger.LogInfo("cant join this lobby");
                return;
            }

            CSteamID lobbyID = new CSteamID(param.m_ulSteamIDLobby);
            Logger.LogInfo($"Joined lobby with ID: {lobbyID}");

            List<PlayerModel> otherPlayers = new List<PlayerModel>();

            int count = SteamMatchmaking.GetNumLobbyMembers(lobbyID);
            for (int i = 0; i < count; i++)
            {
                CSteamID memberID = SteamMatchmaking.GetLobbyMemberByIndex(lobbyID, i);
                string name = SteamFriends.GetFriendPersonaName(memberID);
                Logger.LogInfo($"Player {i}: {name} (id {memberID})");
                otherPlayers.Add(new PlayerModel(memberID.ToString(), name));
            }

            _ = SendContextAsync(new PlayerModel(GetSteamId(), GetUsername()), lobbyID.ToString(), otherPlayers);

            currentLobbyHandler?.Dispose();
            currentLobbyHandler = new LobbyChatUpdateHandler();
        }
    }

    public class LobbyChatUpdateHandler : IDisposable
    {
        private Callback<LobbyChatUpdate_t> _lobbyChatUpdate;

        public LobbyChatUpdateHandler()
        {
            _lobbyChatUpdate = Callback<LobbyChatUpdate_t>.Create(OnLobbyChatUpdate);
        }

        public void Dispose()
        {
            _lobbyChatUpdate?.Dispose();
        }

        private void OnLobbyChatUpdate(LobbyChatUpdate_t callback)
        {
            CSteamID lobbyID = new CSteamID(callback.m_ulSteamIDLobby);
            CSteamID userChanged = new CSteamID(callback.m_ulSteamIDUserChanged);

            EChatMemberStateChange stateChange = (EChatMemberStateChange)callback.m_rgfChatMemberStateChange;

            if (stateChange.HasFlag(EChatMemberStateChange.k_EChatMemberStateChangeEntered))
            {
                string name = SteamFriends.GetFriendPersonaName(userChanged);
                Logger.LogInfo($"Player Joined: {name} (ID: {userChanged})");
            }
            else if (stateChange.HasFlag(EChatMemberStateChange.k_EChatMemberStateChangeLeft) ||
                     stateChange.HasFlag(EChatMemberStateChange.k_EChatMemberStateChangeDisconnected) ||
                     stateChange.HasFlag(EChatMemberStateChange.k_EChatMemberStateChangeKicked) ||
                     stateChange.HasFlag(EChatMemberStateChange.k_EChatMemberStateChangeBanned))
            {
                string name = SteamFriends.GetFriendPersonaName(userChanged);
                Logger.LogInfo($"Player Left: {name} (ID: {userChanged})");
            }
        }
    }

    // handle player passing out, from any source
    [HarmonyPatch(typeof(Character), nameof(Character.RPCA_PassOut))]
    public static class CharacterPassOutPatch
    {
        [HarmonyPostfix]
        private static void CharacterPassOutRPCAPostfix(Character __instance)
        {
            if (__instance != Character.localCharacter) return;

            Logger.LogInfo("Player passed out");
            if (!fallCausedByScoutmaster)
            {
                _ = SendEventAsync("1", "Passed Out", 30, 5000);
            }
            else
            {
                fallCausedByScoutmaster = false;
                _ = SendEventAsync("4", "Thrown By Scoutmaster", 60, 5000);
            }
        }
    }

    // handle instant player death (ex from an item that kills you with no chance for revive)
    [HarmonyPatch(typeof(Character), "DieInstantly")]
    [HarmonyPostfix]
    private static void DieInstantlyPostFix(Character __instance)
    {
        Logger.LogInfo("Player died instantly");
        _ = SendEventAsync("2", "Died Instantly", 30, 10000);
    }


    // handle tracking for when player falls
    [HarmonyPatch(typeof(Character), "Update")]
    class PlayerFallPatch
    {
        [HarmonyPostfix]
        static void PlayerFallPostFix(Character __instance)
        {
            bool isOutOfStamina = __instance.data.currentStamina < 0.005f && __instance.data.extraStamina < 0.001f;
            bool noStaminaFall = isOutOfStamina && __instance.data.avarageVelocity.y < -9.0f;
            bool isDeadlyFall = __instance.data.avarageVelocity.y < -13.0f;
            bool afflictionsInRange = __instance.refs.afflictions.statusSum < 1f;

            if (!__instance.IsLocal)
            {
                return;
            }
            if (!__instance.data.isGrounded && (noStaminaFall || isDeadlyFall))
            {
                if (!isFalling)
                {
                    Logger.LogInfo("Player started Falling at " + Time.time);
                    startedFalling = Time.time;
                    isFalling = true;
                }
            }

            if (__instance.data.isGrounded)
            {
                fallCausedByScoutmaster = false;
            }

            if (__instance.data.isGrounded && (afflictionsInRange || __instance.data.deathTimer > 0.01f))
            {
                if (isFalling)
                {
                    Logger.LogInfo("Player stopped Falling; Landed at after " + (Time.time - startedFalling) + " seconds");
                    isFalling = false;

                    if ((Time.time - startedFalling) >= 1)
                    {
                        // fall was probably large as is worth a clip; this number could use fine tuning later

                        if (__instance.refs.afflictions.statusSum >= 1f)
                        {
                            // player got knocked out from that fall, we can let the pass out function handle the clip so we don't get duplicates
                            return;
                        }

                        if (!fallCausedByScoutmaster)
                        {
                            _ = SendEventAsync("3", "Large Fall", 30, 5000);
                        }
                        else
                        {
                            _ = SendEventAsync("4", "Thrown By Scoutmaster", 60, 5000);
                        }
                    }
                }
            }
        }
        private static float startedFalling = 0;
        private static bool isFalling = false;
    }

    // handle player being flung by scoutmaster
    [HarmonyPatch(typeof(Scoutmaster), nameof(Scoutmaster.RPCA_Throw))]
    public static class ScoutmasterPatch
    {
        [HarmonyPostfix]
        private static void ScoutmasterPatchPostfix(Scoutmaster __instance)
        {
            if (__instance.currentTarget != Character.localCharacter) { return; }

            fallCausedByScoutmaster = true;
            Logger.LogInfo("Player flung by scoutmaster");
        }
    }

    private static string GetSteamId() => SteamUser.GetSteamID().ToString();
    private static string GetUsername() => SteamFriends.GetPersonaName();
    private static int GetMapId()
    {
        var service = GameHandler.GetService<NextLevelService>();
        if (service?.Data == null)
            return -1;
        return service.Data.Value.CurrentLevelIndex;
    }
    private static int GetMapSegment()
    {
        if (MapHandler.Instance == null)
            return -1;
        return (int)MapHandler.Instance.GetCurrentSegment();
    }

    // networking functions stripped from the Medal REPO Plugin for convenience and slightly modified
    internal static async Task SendEventAsync(string eventId, string eventName, int duration, int captureDelayMs)
    {
        if ((Time.time - timeOfLastClip - 5) < 0)
        {
            Logger.LogInfo("Multiple clips really close together blocked");
            return;
        }
        timeOfLastClip = Time.time;

        var jsonPayload = new
        {
            eventId,
            eventName,
            triggerActions = new string[1] { "SaveClip" },
            clipOptions = new
            {
                duration,
                captureDelayMs,
                alertType = "Disabled",
            }
        };
        StringContent? content = new StringContent(JsonConvert.SerializeObject(jsonPayload), Encoding.UTF8, "application/json");
        _ = await httpClient.PostAsync("http://localhost:12665/api/v1/event/invoke", content);
    }

    internal static async Task SendContextAsync(PlayerModel localPlayer, string lobbyId, List<PlayerModel> otherPlayers)
    {
        var jsonPayload = new
        {
            localPlayer,
            serverId = lobbyId,
            matchId = lobbyId,
            otherPlayers
        };
        StringContent? content = new StringContent(JsonConvert.SerializeObject(jsonPayload), Encoding.UTF8, "application/json");
        _ = await httpClient.PostAsync("http://localhost:12665/api/v1/context/submit", content);
    }
}