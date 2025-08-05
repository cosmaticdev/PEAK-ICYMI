// Decompiled with JetBrains decompiler
// Type: MedalRepoPlugin.MedalRepoPlugin
// Assembly: MedalRepoPlugin, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 03342409-9213-4EB8-A5F1-582929B29165
// Assembly location: C:\Users\Mineb\AppData\Local\Medal\recorder-3.1062.0\Plugins\REPO\MedalRepoPlugin.dll

using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

#nullable enable
namespace MedalRepoPlugin;

[BepInPlugin("Medal.MedalRepoPlugin", "MedalRepoPlugin", "1.0")]
public class MedalRepoPlugin : BaseUnityPlugin
{
  private const string PublicKey = "pub_BOpPvZ2UEpQUTDzJ6HSAD62y9DuS0N10";

  internal static MedalRepoPlugin.MedalRepoPlugin Instance { get; private set; }

  internal new static ManualLogSource Logger => MedalRepoPlugin.MedalRepoPlugin.Instance._logger;

  private ManualLogSource _logger => this.Logger;

  internal Harmony? Harmony { get; set; }

  private void Awake()
  {
    MedalRepoPlugin.MedalRepoPlugin.Instance = this;
    ((Component) this).gameObject.transform.parent = (Transform) null;
    ((Object) ((Component) this).gameObject).hideFlags = (HideFlags) 61;
    this.Patch();
    MedalRepoPlugin.MedalRepoPlugin.Logger.LogInfo((object) $"{this.Info.Metadata.GUID} v{this.Info.Metadata.Version} has loaded!");
  }

  internal void Patch()
  {
    if (this.Harmony == null)
    {
      Harmony harmony;
      this.Harmony = harmony = new Harmony(this.Info.Metadata.GUID);
    }
    this.Harmony.PatchAll();
  }

  internal void Unpatch() => this.Harmony?.UnpatchSelf();

  private void Update()
  {
  }

  internal static async Task SendEventAsync(string eventId, string eventName)
  {
    using (HttpClient client = new HttpClient())
    {
      client.DefaultRequestHeaders.Add("publicKey", "pub_BOpPvZ2UEpQUTDzJ6HSAD62y9DuS0N10");
      var jsonPayload = new
      {
        eventId = eventId,
        eventName = eventName,
        triggerActions = new string[1]{ "SaveClip" },
        clipOptions = new
        {
          duration = 30,
          alertType = "Disabled"
        }
      };
      StringContent content = new StringContent(JsonConvert.SerializeObject((object) jsonPayload), Encoding.UTF8, "application/json");
      HttpResponseMessage httpResponseMessage = await client.PostAsync("http://localhost:12665/api/v1/event/invoke", (HttpContent) content);
      jsonPayload = null;
      content = (StringContent) null;
    }
  }

  internal static async Task SendContextAsync(
    PlayerModel localPlayer,
    string lobbyId,
    List<PlayerModel> otherPlayers)
  {
    using (HttpClient client = new HttpClient())
    {
      client.DefaultRequestHeaders.Add("publicKey", "pub_BOpPvZ2UEpQUTDzJ6HSAD62y9DuS0N10");
      var jsonPayload = new
      {
        localPlayer = localPlayer,
        serverId = lobbyId,
        matchId = lobbyId,
        otherPlayers = otherPlayers
      };
      StringContent content = new StringContent(JsonConvert.SerializeObject((object) jsonPayload), Encoding.UTF8, "application/json");
      HttpResponseMessage httpResponseMessage = await client.PostAsync("http://localhost:12665/api/v1/context/submit", (HttpContent) content);
      jsonPayload = null;
      content = (StringContent) null;
    }
  }
}
