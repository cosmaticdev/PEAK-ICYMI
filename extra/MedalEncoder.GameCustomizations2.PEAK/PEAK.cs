using System;
using System.Threading.Tasks;

#nullable disable
namespace MedalEncoder.GameCustomizations2.PEAK;

[GameCustomization("1NR4_vwLDkr")]
internal class PEAK : BaseGameCustomization
{
  public const string GameCategoryID = "1NR4_vwLDkr";
  internal const string DefaultGameDir = "steamapps\\common\\PEAK";
  internal const string ExecutablePath = "steamapps\\common\\PEAK\\PEAK.exe";

  public PEAK()
    : base("1NR4_vwLDkr", BaseGameCustomization.EventDetectionType.RealTime, new EventFormatter(PEAKFormatter.FormatClipMetadata))
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
    PluginManager.Instance.RegisterPlugin("1NR4_vwLDkr", new PluginManager.Actions((Func<MedalEncoder.GameCustomizations2.PluginStatus>) (() => !PEAKPluginUtils.CheckPluginStatus() ? MedalEncoder.GameCustomizations2.PluginStatus.Available : MedalEncoder.GameCustomizations2.PluginStatus.Installed), new Action(PEAKPluginUtils.InstallPlugin), new Action(PEAKPluginUtils.UninstallPlugin)));
  }
}
