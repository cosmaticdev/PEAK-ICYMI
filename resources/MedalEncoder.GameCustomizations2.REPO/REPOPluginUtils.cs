// Decompiled with JetBrains decompiler
// Type: MedalEncoder.GameCustomizations2.REPO.REPOPluginUtils
// Assembly: MedalEncoder, Version=3.1062.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 707522FE-0E3F-4042-8489-C5CD5D666AE8
// Assembly location: C:\Users\Mineb\AppData\Local\Medal\recorder-3.1062.0\MedalEncoder.exe

using RESTService;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Reflection;

#nullable disable
namespace MedalEncoder.GameCustomizations2.REPO;

internal class REPOPluginUtils
{
  private static readonly string _recorderBaseDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
  private static readonly string _recorderPluginsDir = Path.Combine(REPOPluginUtils._recorderBaseDir, "Plugins");
  private static readonly object _lock = new object();

  public static bool CheckPluginStatus()
  {
    if (!REPOPluginUtils.GameInstalled())
      return false;
    REPOPluginUtils.PluginStatus pluginStatus1 = REPOPluginUtils.ThunderstoreModsInstalled();
    REPOPluginUtils.PluginStatus pluginStatus2 = REPOPluginUtils.GameModInstalled();
    if (pluginStatus1 == REPOPluginUtils.PluginStatus.MODS_INSTALLED && pluginStatus2 == REPOPluginUtils.PluginStatus.MODS_INSTALLED)
      return true;
    return pluginStatus1 == REPOPluginUtils.PluginStatus.THUNDERSTORE_NOT_INSTALLED && pluginStatus2 == REPOPluginUtils.PluginStatus.MODS_INSTALLED;
  }

  public static void InstallPlugin()
  {
    lock (REPOPluginUtils._lock)
    {
      try
      {
        int num = REPOPluginUtils.InstallPluginInThunderstore() ? 1 : 0;
        bool flag = REPOPluginUtils.InstallPluginInGame();
        if (num != 0 && flag)
          return;
        EventLog.LogWarning("Error installing REPO plugin. Uninstalling...");
        REPOPluginUtils.UninstallPlugin();
      }
      catch (Exception ex)
      {
        EventLog.LogWarning("Error installing plugin: " + ex.Message);
        REPOPluginUtils.UninstallPlugin();
      }
    }
  }

  public static void UninstallPlugin()
  {
    lock (REPOPluginUtils._lock)
    {
      try
      {
        REPOPluginUtils.UninstallPluginInThunderstore();
        REPOPluginUtils.UninstallPluginInGame();
      }
      catch (Exception ex)
      {
        EventLog.LogWarning("Error uninstalling plugin: " + ex.Message);
      }
    }
  }

  private static void UninstallPluginInThunderstore()
  {
    string path1 = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Thunderstore Mod Manager", "DataFolder", "REPO");
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
        string path = Path.Combine(directory, "BepInEx", "plugins", "MedalTV-MedalRepoPlugin");
        if (Directory.Exists(path))
          Directory.Delete(path, true);
      }
    }
  }

  private static void UninstallPluginInGame()
  {
    string str = Path.Combine(GameFilePaths.GetGameLibraryBasePath("steamapps\\common\\REPO", "steamapps\\common\\REPO\\REPO.exe"), "steamapps\\common\\REPO");
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
      string path2 = Path.Combine(str, "BepInEx", "plugins", "MedalTV-MedalRepoPlugin");
      if (!Directory.Exists(path2))
        return;
      Directory.Delete(path2, true);
    }
  }

  private static bool EnsureBepInExInstalled(string baseDir)
  {
    if (!Directory.Exists(Path.Combine(baseDir, "BepInEx")))
    {
      string sourceArchiveFileName = Environment.Is64BitOperatingSystem ? Path.Combine(REPOPluginUtils._recorderPluginsDir, "BepInEx", "BepInEx_x64_5.4.21.0.zip") : Path.Combine(REPOPluginUtils._recorderPluginsDir, "BepInEx", "BepInEx_x86_5.4.21.0.zip");
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
    string sourceFileName = Path.Combine(REPOPluginUtils._recorderPluginsDir, "REPO", "MedalRepoPlugin.dll");
    string str1 = Path.Combine(baseDir, "BepInEx", "plugins", "MedalTV-MedalRepoPlugin");
    string str2 = Path.Combine(str1, "MedalRepoPlugin.dll");
    if (!Directory.Exists(str1))
      Directory.CreateDirectory(str1);
    string destFileName = str2;
    File.Copy(sourceFileName, destFileName, true);
  }

  private static bool InstallPluginInGame()
  {
    string str = Path.Combine(GameFilePaths.GetGameLibraryBasePath("steamapps\\common\\REPO", "steamapps\\common\\REPO\\REPO.exe"), "steamapps\\common\\REPO");
    if (!Directory.Exists(str))
      return false;
    REPOPluginUtils.EnsureBepInExInstalled(str);
    REPOPluginUtils.InstallPluginToDirectory(str);
    return true;
  }

  private static bool InstallPluginInThunderstore()
  {
    try
    {
      string path1 = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Thunderstore Mod Manager", "DataFolder", "REPO");
      if (!Directory.Exists(Path.Combine(path1, "profiles")))
        return true;
      foreach (string directory in Directory.GetDirectories(Path.Combine(path1, "profiles")))
      {
        REPOPluginUtils.EnsureBepInExInstalled(directory);
        REPOPluginUtils.InstallPluginToDirectory(directory);
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
    return Directory.Exists(Path.Combine(GameFilePaths.GetGameLibraryBasePath("steamapps\\common\\REPO", "steamapps\\common\\REPO\\REPO.exe"), "steamapps\\common\\REPO"));
  }

  private static REPOPluginUtils.PluginStatus ThunderstoreModsInstalled()
  {
    string path1_1 = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Thunderstore Mod Manager", "DataFolder", "REPO");
    if (!Directory.Exists(Path.Combine(path1_1, "profiles")))
      return REPOPluginUtils.PluginStatus.THUNDERSTORE_NOT_INSTALLED;
    List<string> stringList = new List<string>();
    foreach (string directory in Directory.GetDirectories(Path.Combine(path1_1, "profiles")))
    {
      string str = Path.Combine(directory, "BepInEx");
      if (!Directory.Exists(str))
        return REPOPluginUtils.PluginStatus.MODS_NOT_INSTALLED;
      stringList.Add(Path.Combine(str, "plugins"));
    }
    foreach (string path1_2 in stringList)
    {
      if (!File.Exists(Path.Combine(path1_2, "MedalTV-MedalRepoPlugin", "MedalRepoPlugin.dll")))
        return REPOPluginUtils.PluginStatus.MODS_NOT_INSTALLED;
    }
    return REPOPluginUtils.PluginStatus.MODS_INSTALLED;
  }

  private static REPOPluginUtils.PluginStatus GameModInstalled()
  {
    return !File.Exists(Path.Combine(Path.Combine(Path.Combine(GameFilePaths.GetGameLibraryBasePath("steamapps\\common\\REPO", "steamapps\\common\\REPO\\REPO.exe"), "steamapps\\common\\REPO"), "BepInEx", "plugins", "MedalTV-MedalRepoPlugin"), "MedalRepoPlugin.dll")) ? REPOPluginUtils.PluginStatus.MODS_NOT_INSTALLED : REPOPluginUtils.PluginStatus.MODS_INSTALLED;
  }

  public enum PluginStatus
  {
    THUNDERSTORE_NOT_INSTALLED,
    MODS_NOT_INSTALLED,
    MODS_INSTALLED,
  }
}
