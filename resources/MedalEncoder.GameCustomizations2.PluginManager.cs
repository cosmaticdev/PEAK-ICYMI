// Decompiled with JetBrains decompiler
// Type: MedalEncoder.GameCustomizations2.PluginManager
// Assembly: MedalEncoder, Version=3.1068.0.0, Culture=neutral, PublicKeyToken=null
// MVID: AA7E5C47-99B2-4D9F-BEFA-1E55C8031E53
// Assembly location: C:\Users\cael\AppData\Local\Medal\recorder-3.1068.0\MedalEncoder.exe

using RESTService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

#nullable disable
namespace MedalEncoder.GameCustomizations2;

internal class PluginManager
{
  private Dictionary<string, PluginManager.Actions> _plugins = new Dictionary<string, PluginManager.Actions>();
  private object _pluginsLock = new object();

  private PluginManager()
  {
  }

  public static PluginManager Instance { get; } = new PluginManager();

  public string[] RegisteredCategories
  {
    get
    {
      lock (this._pluginsLock)
        return this._plugins.Keys.ToArray<string>();
    }
  }

  public string[] InstalledCategories
  {
    get
    {
      lock (this._pluginsLock)
        return this._plugins.Where<KeyValuePair<string, PluginManager.Actions>>((Func<KeyValuePair<string, PluginManager.Actions>, bool>) (t => t.Value.GetStatus() == PluginStatus.Installed)).Select<KeyValuePair<string, PluginManager.Actions>, string>((Func<KeyValuePair<string, PluginManager.Actions>, string>) (t => t.Key)).ToArray<string>();
    }
  }

  public bool HasPlugin(string categoryID)
  {
    lock (this._pluginsLock)
      return this._plugins.ContainsKey(categoryID);
  }

  public async Task<bool> InstallPluginAsync(string categoryID)
  {
    PluginManager.Actions actions;
    lock (this._pluginsLock)
    {
      if (!this._plugins.TryGetValue(categoryID, out actions))
        return false;
    }
    try
    {
      await Task.Run((Action) (() => actions.Install()));
      return true;
    }
    catch (Exception ex)
    {
      EventLog.LogWarning($"Failed to install plugin: {ex}");
      return false;
    }
  }

  public async Task<bool> UninstallPluginAsync(string categoryID)
  {
    PluginManager.Actions actions;
    lock (this._pluginsLock)
    {
      if (!this._plugins.TryGetValue(categoryID, out actions))
        return false;
    }
    try
    {
      await Task.Run((Action) (() => actions.Uninstall()));
      return true;
    }
    catch (Exception ex)
    {
      EventLog.LogWarning($"Failed to uninstall plugin: {ex}");
      return false;
    }
  }

  public bool RegisterPlugin(string categoryID, PluginManager.Actions actions)
  {
    lock (this._pluginsLock)
    {
      try
      {
        this._plugins.Add(categoryID, actions);
        return true;
      }
      catch (ArgumentException ex)
      {
        EventLog.LogCritical("Failed to register plugin: already exists", "Plugin for category " + categoryID);
        return false;
      }
    }
  }

  public bool UnregisterPlugin(string categoryID)
  {
    lock (this._pluginsLock)
      return this._plugins.Remove(categoryID);
  }

  public class Actions
  {
    public Func<PluginStatus> GetStatus { get; private set; }

    public Action Install { get; private set; }

    public Action Uninstall { get; private set; }

    public Actions(Func<PluginStatus> status, Action install, Action uninstall)
    {
      this.GetStatus = status;
      this.Install = install;
      this.Uninstall = uninstall;
    }
  }
}
