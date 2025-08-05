// Decompiled with JetBrains decompiler
// Type: MedalRepoPlugin.SemiFuncPatch
// Assembly: MedalRepoPlugin, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 03342409-9213-4EB8-A5F1-582929B29165
// Assembly location: C:\Users\Mineb\AppData\Local\Medal\recorder-3.1062.0\Plugins\REPO\MedalRepoPlugin.dll

using HarmonyLib;
using Steamworks;
using Steamworks.Data;
using System;
using System.Collections.Generic;

#nullable disable
namespace MedalRepoPlugin;

[HarmonyPatch(typeof (SemiFunc), "OnSceneSwitch")]
public static class SemiFuncPatch
{
  [HarmonyPostfix]
  public static void OnSceneSwitchPostfix(bool _gameOver, bool _leaveGame)
  {
    MedalRepoPlugin.MedalRepoPlugin.Logger.LogInfo((object) "OnSceneSwitch called");
    List<PlayerModel> otherPlayers = new List<PlayerModel>();
    PlayerModel localPlayer = new PlayerModel("", "");
    try
    {
      foreach (PlayerAvatar playerAvatar in SemiFunc.PlayerGetAll())
      {
        PlayerModel playerModel = new PlayerModel(SemiFunc.PlayerGetSteamID(playerAvatar), SemiFunc.PlayerGetName(playerAvatar));
        if (playerAvatar.isLocal)
          localPlayer = playerModel;
        else
          otherPlayers.Add(playerModel);
      }
    }
    catch (Exception ex)
    {
      MedalRepoPlugin.MedalRepoPlugin.Logger.LogError((object) ("Error getting player list: " + ex.Message));
    }
    foreach (PlayerModel playerModel in otherPlayers)
      MedalRepoPlugin.MedalRepoPlugin.Logger.LogInfo((object) ("Steam ID found: " + playerModel.playerId));
    Lobby? nullable = (Lobby?) AccessTools.Field(typeof (SteamManager), "currentLobby")?.GetValue((object) SteamManager.instance);
    SteamId steamId = (SteamId) AccessTools.Property(typeof (Lobby), "Id").GetValue((object) nullable);
    string lobbyId = steamId.ToString();
    if (lobbyId == null || !(lobbyId != "0"))
      return;
    MedalRepoPlugin.MedalRepoPlugin.Logger.LogInfo((object) $"Lobby ID found: {steamId}");
    MedalRepoPlugin.MedalRepoPlugin.SendContextAsync(localPlayer, lobbyId, otherPlayers);
  }
}
