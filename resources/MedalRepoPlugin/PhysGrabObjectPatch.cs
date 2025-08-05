// Decompiled with JetBrains decompiler
// Type: MedalRepoPlugin.PhysGrabObjectPatch
// Assembly: MedalRepoPlugin, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 03342409-9213-4EB8-A5F1-582929B29165
// Assembly location: C:\Users\Mineb\AppData\Local\Medal\recorder-3.1062.0\Plugins\REPO\MedalRepoPlugin.dll

using HarmonyLib;

#nullable enable
namespace MedalRepoPlugin;

[HarmonyPatch(typeof (PhysGrabObject), "DestroyPhysGrabObjectRPC")]
public static class PhysGrabObjectPatch
{
  [HarmonyPostfix]
  public static void DestroyPhysGrabObjectRPCPostfix(PhysGrabObject __instance)
  {
    if (!(__instance.grabbed & __instance.heldByLocalPlayer))
      return;
    MedalRepoPlugin.MedalRepoPlugin.Logger.LogInfo((object) "item break triggered");
    MedalRepoPlugin.MedalRepoPlugin.SendEventAsync("2", "Item Break");
  }
}
