// Decompiled with JetBrains decompiler
// Type: MedalEncoder.GameCustomizations2.REPO.REPO
// Assembly: MedalEncoder, Version=3.1062.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 707522FE-0E3F-4042-8489-C5CD5D666AE8
// Assembly location: C:\Users\Mineb\AppData\Local\Medal\recorder-3.1062.0\MedalEncoder.exe

using System;
using System.Threading.Tasks;

#nullable disable
namespace MedalEncoder.GameCustomizations2.REPO;

[GameCustomization("1HfH5qHqa1y")]
internal class REPO : BaseGameCustomization
{
  public const string GameCategoryID = "1HfH5qHqa1y";
  internal const string DefaultGameDir = "steamapps\\common\\REPO";
  internal const string ExecutablePath = "steamapps\\common\\REPO\\REPO.exe";

  public REPO()
    : base("1HfH5qHqa1y", BaseGameCustomization.EventDetectionType.RealTime, new EventFormatter(REPOFormatter.FormatClipMetadata))
  {
  }

  protected override Task OnGameStarted(TargetWindow activeGame)
  {
    this.EMS.Padding = TimeSpan.FromSeconds(15.0);
    this.EMS.EventWindow = TimeSpan.FromSeconds(15.0);
    return Task.CompletedTask;
  }

  protected override Task OnGameStopped() => Task.CompletedTask;

  public static void OnAppStart()
  {
    PluginManager.Instance.RegisterPlugin("1HfH5qHqa1y", new PluginManager.Actions((Func<MedalEncoder.GameCustomizations2.PluginStatus>) (() => !REPOPluginUtils.CheckPluginStatus() ? MedalEncoder.GameCustomizations2.PluginStatus.Available : MedalEncoder.GameCustomizations2.PluginStatus.Installed), new Action(REPOPluginUtils.InstallPlugin), new Action(REPOPluginUtils.UninstallPlugin)));
  }
}
