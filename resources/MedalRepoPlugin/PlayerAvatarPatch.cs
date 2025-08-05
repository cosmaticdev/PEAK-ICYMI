// Decompiled with JetBrains decompiler
// Type: MedalRepoPlugin.PlayerAvatarPatch
// Assembly: MedalRepoPlugin, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 03342409-9213-4EB8-A5F1-582929B29165
// Assembly location: C:\Users\Mineb\AppData\Local\Medal\recorder-3.1062.0\Plugins\REPO\MedalRepoPlugin.dll

using HarmonyLib;

#nullable enable
namespace MedalRepoPlugin;

[HarmonyPatch(typeof (PlayerAvatar), "PlayerDeath")]
public static class PlayerAvatarPatch
{
  [HarmonyPostfix]
  public static void PlayerDeathPostfix(PlayerAvatar __instance)
  {
    MedalRepoPlugin.MedalRepoPlugin.Logger.LogInfo((object) "death triggered");
    MedalRepoPlugin.MedalRepoPlugin.SendEventAsync("1", "Death");
  }
}
