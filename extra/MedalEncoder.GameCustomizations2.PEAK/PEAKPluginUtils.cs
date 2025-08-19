using RESTService;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Reflection;

#nullable disable
namespace MedalEncoder.GameCustomizations2.PEAK;

internal class PEAKPluginUtils
{
  private static readonly string _recorderBaseDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
  private static readonly string _recorderPluginsDir = Path.Combine(PEAKPluginUtils._recorderBaseDir, "Plugins");
  private static readonly object _lock = new object();

  public static bool CheckPluginStatus()
  {
    if (!PEAKPluginUtils.GameInstalled())
      return false;
    PEAKPluginUtils.PluginStatus pluginStatus1 = PEAKPluginUtils.ThunderstoreModsInstalled();
    PEAKPluginUtils.PluginStatus pluginStatus2 = PEAKPluginUtils.GameModInstalled();
    if (pluginStatus1 == PEAKPluginUtils.PluginStatus.MODS_INSTALLED && pluginStatus2 == PEAKPluginUtils.PluginStatus.MODS_INSTALLED)
      return true;
    return pluginStatus1 == PEAKPluginUtils.PluginStatus.THUNDERSTORE_NOT_INSTALLED && pluginStatus2 == PEAKPluginUtils.PluginStatus.MODS_INSTALLED;
  }

  public static void InstallPlugin()
  {
    lock (PEAKPluginUtils._lock)
    {
      try
      {
        int num = PEAKPluginUtils.InstallPluginInThunderstore() ? 1 : 0;
        bool flag = PEAKPluginUtils.InstallPluginInGame();
        if (num != 0 && flag)
          return;
        EventLog.LogWarning("Error installing PEAK plugin. Uninstalling...");
        PEAKPluginUtils.UninstallPlugin();
      }
      catch (Exception ex)
      {
        EventLog.LogWarning("Error installing plugin: " + ex.Message);
        PEAKPluginUtils.UninstallPlugin();
      }
    }
  }

  public static void UninstallPlugin()
  {
    lock (PEAKPluginUtils._lock)
    {
      try
      {
        PEAKPluginUtils.UninstallPluginInThunderstore();
        PEAKPluginUtils.UninstallPluginInGame();
      }
      catch (Exception ex)
      {
        EventLog.LogWarning("Error uninstalling plugin: " + ex.Message);
      }
    }
  }

  private static void UninstallPluginInThunderstore()
  {
    string path1 = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Thunderstore Mod Manager", "DataFolder", "PEAK");
    if (!Directory.Exists(Path.Combine(path1, "profiles")))
      return;
    foreach (string directory in Directory.GetDirectories(Path.Combine(path1, "profiles")))
    {
      if (File.Exists(Path.Combine(directory, "MedalEncoder.txt")))
      {
        Directory.Delete(Path.Combine(directory, "BepInEx"), true);
        File.Delete(Path.Combine(directory, "MedalEncoder.txt"));
        File.Delete(Path.Combine(directory, "doorstop_config.ini"));
        File.Delete(Path.Combine(directory, "changelog.txt"));
        File.Delete(Path.Combine(directory, "winhttp.dll"));
      }
      else
      {
        string path = Path.Combine(directory, "BepInEx", "plugins", "MedalTV-MedalPEAKPlugin");
        if (Directory.Exists(path))
          Directory.Delete(path, true);
      }
    }
  }

  private static void UninstallPluginInGame()
  {
    string str = Path.Combine(GameFilePaths.GetGameLibraryBasePath("steamapps\\common\\PEAK", "steamapps\\common\\PEAK\\PEAK.exe"), "steamapps\\common\\PEAK");
    if (!Directory.Exists(str))
      return;
    string path1 = Path.Combine(str, "MedalEncoder.txt");
    if (File.Exists(path1))
    {
      Directory.Delete(Path.Combine(str, "BepInEx"), true);
      File.Delete(path1);
      File.Delete(Path.Combine(str, "doorstop_config.ini"));
      File.Delete(Path.Combine(str, "changelog.txt"));
      File.Delete(Path.Combine(str, "winhttp.dll"));
    }
    else
    {
      string path2 = Path.Combine(str, "BepInEx", "plugins", "MedalTV-MedalPEAKPlugin");
      if (!Directory.Exists(path2))
        return;
      Directory.Delete(path2, true);
    }
  }

  private static bool EnsureBepInExInstalled(string baseDir)
  {
    if (!Directory.Exists(Path.Combine(baseDir, "BepInEx")))
    {
      string sourceArchiveFileName = Environment.Is64BitOperatingSystem ? Path.Combine(PEAKPluginUtils._recorderPluginsDir, "BepInEx", "BepInEx_x64_5.4.21.0.zip") : Path.Combine(PEAKPluginUtils._recorderPluginsDir, "BepInEx", "BepInEx_x86_5.4.21.0.zip");
      File.Delete(Path.Combine(baseDir, "changelog.txt"));
      File.Delete(Path.Combine(baseDir, "doorstop_config.ini"));
      File.Delete(Path.Combine(baseDir, "winhttp.dll"));
      string destinationDirectoryName = baseDir;
      ZipFile.ExtractToDirectory(sourceArchiveFileName, destinationDirectoryName);
      File.Create(Path.Combine(baseDir, "MedalEncoder.txt")).Dispose();
    }
    return true;
  }

  private static void InstallPluginToDirectory(string baseDir)
  {
    string sourceFileName = Path.Combine(PEAKPluginUtils._recorderPluginsDir, "PEAK", "MedalPeakPlugin.dll");
    string str1 = Path.Combine(baseDir, "BepInEx", "plugins", "MedalTV-MedalPEAKPlugin");
    string str2 = Path.Combine(str1, "MedalPEAKPlugin.dll");
    if (!Directory.Exists(str1))
      Directory.CreateDirectory(str1);
    string destFileName = str2;
    File.Copy(sourceFileName, destFileName, true);
  }

  private static bool InstallPluginInGame()
  {
    string str = Path.Combine(GameFilePaths.GetGameLibraryBasePath("steamapps\\common\\PEAK", "steamapps\\common\\PEAK\\PEAK.exe"), "steamapps\\common\\PEAK");
    if (!Directory.Exists(str))
      return false;
    PEAKPluginUtils.EnsureBepInExInstalled(str);
    PEAKPluginUtils.InstallPluginToDirectory(str);
    return true;
  }

  private static bool InstallPluginInThunderstore()
  {
    try
    {
      string path1 = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Thunderstore Mod Manager", "DataFolder", "PEAK");
      if (!Directory.Exists(Path.Combine(path1, "profiles")))
        return true;
      foreach (string directory in Directory.GetDirectories(Path.Combine(path1, "profiles")))
      {
        PEAKPluginUtils.EnsureBepInExInstalled(directory);
        PEAKPluginUtils.InstallPluginToDirectory(directory);
      }
    }
    catch (Exception ex)
    {
      EventLog.LogWarning("Error installing plugin in thunderstore: " + ex.Message);
      return false;
    }
    return true;
  }

  private static bool GameInstalled()
  {
    return Directory.Exists(Path.Combine(GameFilePaths.GetGameLibraryBasePath("steamapps\\common\\PEAK", "steamapps\\common\\PEAK\\PEAK.exe"), "steamapps\\common\\PEAK"));
  }

  private static PEAKPluginUtils.PluginStatus ThunderstoreModsInstalled()
  {
    string path1_1 = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Thunderstore Mod Manager", "DataFolder", "PEAK");
    if (!Directory.Exists(Path.Combine(path1_1, "profiles")))
      return PEAKPluginUtils.PluginStatus.THUNDERSTORE_NOT_INSTALLED;
    List<string> stringList = new List<string>();
    foreach (string directory in Directory.GetDirectories(Path.Combine(path1_1, "profiles")))
    {
      string str = Path.Combine(directory, "BepInEx");
      if (!Directory.Exists(str))
        return PEAKPluginUtils.PluginStatus.MODS_NOT_INSTALLED;
      stringList.Add(Path.Combine(str, "plugins"));
    }
    foreach (string path1_2 in stringList)
    {
      if (!File.Exists(Path.Combine(path1_2, "MedalTV-MedalPEAKPlugin", "MedalPEAKPlugin.dll")))
        return PEAKPluginUtils.PluginStatus.MODS_NOT_INSTALLED;
    }
    return PEAKPluginUtils.PluginStatus.MODS_INSTALLED;
  }

  private static PEAKPluginUtils.PluginStatus GameModInstalled()
  {
    return !File.Exists(Path.Combine(Path.Combine(Path.Combine(GameFilePaths.GetGameLibraryBasePath("steamapps\\common\\PEAK", "steamapps\\common\\PEAK\\PEAK.exe"), "steamapps\\common\\PEAK"), "BepInEx", "plugins", "MedalTV-MedalPEAKPlugin"), "MedalPEAKPlugin.dll")) ? PEAKPluginUtils.PluginStatus.MODS_NOT_INSTALLED : PEAKPluginUtils.PluginStatus.MODS_INSTALLED;
  }

  public enum PluginStatus
  {
    THUNDERSTORE_NOT_INSTALLED,
    MODS_NOT_INSTALLED,
    MODS_INSTALLED,
  }
}
