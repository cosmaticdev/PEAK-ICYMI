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

#nullable enable
namespace MedalPeakPlugin;

[BepInPlugin("cosmatic.MedalPeakPlugin", "MedalPeakPlugin", "1.0")]
public class MedalPeakPlugin : BaseUnityPlugin
{
    internal static MedalPeakPlugin? Instance { get; private set; }
    internal Harmony? Harmony { get; set; }
    internal static ManualLogSource Log { get; private set; } = null!;

    internal static float timeOfLastClip = 0;
    internal static bool fallCausedByScoutmaster = false;

    private void Awake()
    {
        Instance = this;
        Log = Logger;

        this.gameObject.transform.parent = null;
        this.gameObject.hideFlags = (HideFlags)61;
        this.Patch();
        //Log.LogInfo($"Plugin has loaded!");
        Debug.Log("Plugin Loaded------------------------");
    }

    internal void Patch()
    {
        if (this.Harmony == null)
        {
            Harmony harmony;
            this.Harmony = harmony = new Harmony("cosmatic.MedalPeakPlugin");
        }
        this.Harmony.PatchAll();
    }

    internal void Unpatch() => this.Harmony?.UnpatchSelf();

    private void Update()
    {
    }

    // handle player passing out, from any source
    [HarmonyPatch(typeof(Character), nameof(Character.RPCA_PassOut))]
    public static class CharacterPassOutPatch
    {
        [HarmonyPostfix]
        private static async Task CharacterPassOutRPCAPostfix(Character __instance)
        {
            if (__instance != Character.localCharacter) return;

            var steamId = GetSteamId();
            var mapId = GetMapId();
            var mapSegment = GetMapSegment();

            Debug.Log("Player passed out, steamid: " + steamId + ", mapid: " + mapId + ", mapsegment: " + mapSegment);
            if (!fallCausedByScoutmaster)
            {
                await SendEventAsync("1", "Player Passed Out", 30, 5000);
            }
            else
            {
                fallCausedByScoutmaster = false;
                await SendEventAsync("4", "Player Thrown By Scoutmaster", 60, 5000);
            }
        }
    }

    // handle instant player death (ex from an item that kills you with no chance for revive)
    [HarmonyPatch(typeof(Character), "DieInstantly")]
    [HarmonyPostfix]
    private static async Task DieInstantlyPostFix(CharacterMovement __instance)
    {
        Debug.Log("Player died instantly");

        var steamId = GetSteamId();
        var mapId = GetMapId();
        var mapSegment = GetMapSegment();
        await SendEventAsync("2", "Player Died Instantly", 30, 10000);
    }


    // handle tracking for when player falls
    [HarmonyPatch(typeof(Character), "Update")]
    class PlayerFallPatch
    {
        [HarmonyPostfix]
        static async Task PlayerFallPostFix(Character __instance)
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
                    Debug.Log("Player started Falling at " + Time.time + "------------");
                    startedFalling = Time.time;
                    isFalling = true;
                }
            }

            if (__instance.data.isGrounded && (afflictionsInRange || __instance.data.deathTimer > 0.01f))
            {
                if (isFalling)
                {
                    Debug.Log("Player stopped Falling; Landed at " + Time.time + " after " + (Time.time - startedFalling) + " seconds------------");
                    isFalling = false;

                    if ((Time.time - startedFalling) >= 1)
                    {
                        // fall was probably large as is worth a clip; this number could use fine tuning later

                        if (__instance.refs.afflictions.statusSum >= 1f)
                        {
                            // player got knocked out from that fall, we can let the pass out function handle the clip so we don't get duplicates
                            return;
                        }

                        var steamId = GetSteamId();
                        var mapId = GetMapId();
                        var mapSegment = GetMapSegment();

                        if (!fallCausedByScoutmaster)
                        {
                            await SendEventAsync("3", "Large Fall", 30, 5000);
                        }
                        else
                        {
                            await SendEventAsync("4", "Player Thrown By Scoutmaster", 60, 5000);
                        }
                    }

                    fallCausedByScoutmaster = false; // further falls are definetly not by the scoutmaster
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

            var steamId = GetSteamId();
            var mapId = GetMapId();
            var mapSegment = GetMapSegment();

            Debug.Log("Player flung by scoutmaster, steamid: " + steamId + ", mapid: " + mapId + ", mapsegment: " + mapSegment);
        }
    }

    private static string GetSteamId() => SteamUser.GetSteamID().ToString();
    private static int GetMapId() => GameHandler.GetService<NextLevelService>().Data.Value.CurrentLevelIndex;
    private static int GetMapSegment() => (int)MapHandler.Instance.GetCurrentSegment();

    // networking functions stripped from the Medal REPO Plugin for convenience and slightly modified
    internal static async Task SendEventAsync(string eventId, string eventName, int duration, int captureDelayMs)
    {
        using (HttpClient client = new HttpClient())
        {
            if (Time.time - 5 > timeOfLastClip)
            {
                Debug.Log("Multiple clips really close together blocked");
                return;
            }
            timeOfLastClip = Time.time;

            client.DefaultRequestHeaders.Add("publicKey", "pub_7yyLREtjlmTJeGtpI8wWR9NxpIkuvTF1");
            var jsonPayload = new
            {
                eventId = eventId,
                eventName = eventName,
                triggerActions = new string[1] { "SaveClip" },
                clipOptions = new
                {
                    duration,
                    captureDelayMs,
                    alertType = "SoundOnly",//alertType = "Disabled",
                }
            };
            StringContent? content = new StringContent(JsonConvert.SerializeObject(jsonPayload), Encoding.UTF8, "application/json");
            HttpResponseMessage httpResponseMessage = await client.PostAsync("http://localhost:12665/api/v1/event/invoke", content);
            jsonPayload = null;
            content = null;
        }
    }

    internal static async Task SendContextAsync(
      PlayerModel localPlayer,
      string lobbyId,
      List<PlayerModel> otherPlayers)
    {
        using (HttpClient client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("publicKey", "pub_7yyLREtjlmTJeGtpI8wWR9NxpIkuvTF1");
            var jsonPayload = new
            {
                localPlayer = localPlayer,
                serverId = lobbyId,
                matchId = lobbyId,
                otherPlayers = otherPlayers
            };
            StringContent? content = new StringContent(JsonConvert.SerializeObject(jsonPayload), Encoding.UTF8, "application/json");
            HttpResponseMessage httpResponseMessage = await client.PostAsync("http://localhost:12665/api/v1/context/submit", content);
            jsonPayload = null;
            content = null;
        }
    }
}