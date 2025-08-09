Medal stores loader code for their REPO ICYMI plugin in its local/Medal/recorder-.../MedalEncoder.exe package (specifically, MedalEncoder.GameCustomizations2.{game})

The methods vary game-to-game, but since PEAK & REPO are both developed in Unity, their injection and loading processes should be applicable to each other. 

Specifically, I found that Medal uses a Unity modding framework [BepInEx](https://github.com/BepInEx/BepInEx), which greatly simplifies things development-wise as it is standardized across tons of different games (including PEAK, epic)

If you wish to build on your own or develop something similar [this guide](https://docs.bepinex.dev/articles/user_guide/installation/index.html) was a big help for me.

Reading through the installation code for repo (which ive put in [resources/MedalEncoder.GameCustomizations2.REPO/REPOPluginUtils.cs](https://github.com/cosmaticdev/PEAK-ICYMI/blob/master/resources/MedalEncoder.GameCustomizations2.REPO/REPOPluginUtils.cs)) I found how Medal loads the BepInEx library; simply dropping the files into the base of the repo steam folder.

```cs
private static bool InstallPluginInGame()
  {
    string str = Path.Combine(GameFilePaths.GetGameLibraryBasePath("steamapps\\common\\REPO", "steamapps\\common\\REPO\\REPO.exe"), "steamapps\\common\\REPO"); // determine the REPO installation location
    if (!Directory.Exists(str)) //check if the game is even installed and the location is real
      return false;
    REPOPluginUtils.EnsureBepInExInstalled(str); // makes sure the modding framework is installed
    REPOPluginUtils.InstallPluginToDirectory(str); // installs the Medal plugin to the framework that just got added
    return true;
  }

private static bool EnsureBepInExInstalled(string baseDir)
  {
    if (!Directory.Exists(Path.Combine(baseDir, "BepInEx")))
    {
      string sourceArchiveFileName = Environment.Is64BitOperatingSystem ? Path.Combine(REPOPluginUtils._recorderPluginsDir, "BepInEx", "BepInEx_x64_5.4.21.0.zip") : Path.Combine(REPOPluginUtils._recorderPluginsDir, "BepInEx", "BepInEx_x86_5.4.21.0.zip"); // install the right BepInEx version for the pc archetecture (x64 or x86)

      // the next few lines wipe old existing or conflicting files to allow for a clean installation
      File.Delete(Path.Combine(baseDir, "changelog.txt"));
      File.Delete(Path.Combine(baseDir, "doorstop_config.ini")); 
      File.Delete(Path.Combine(baseDir, "winhttp.dll"));

      string destinationDirectoryName = baseDir;
      ZipFile.ExtractToDirectory(sourceArchiveFileName, destinationDirectoryName); // installs a fresh and updated copy of everything
      File.Create(Path.Combine(baseDir, "MedalEncoder.txt")).Dispose(); // creates a branded log file
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
    File.Copy(sourceFileName, destFileName, true); // copies over the ICYMI plugin DLL to the BepInEx folder that was just created inside the games source files
  }
```
please note the above in-code comments were added by me and not the Medal team.

So this tells us exactly where they place their files and how they have them run! Theoretically, if we repeat all of these steps manually we could theoretically replace MedalRepoPlugin.dll with our own MedalPeakPlugin.dll and execute our own code.

But first we need to check out what's actually in that dll. It's contents have been placed in [resources/MedalRepoPlugin](https://github.com/cosmaticdev/PEAK-ICYMI/tree/master/resources/MedalRepoPlugin) so you don't need to decompile Medal yourself. Most of the plugin scripts seem to check for in-game events and actions and then act acordingly, and additionally [MedalRepoPlugin.cs](https://github.com/cosmaticdev/PEAK-ICYMI/blob/master/resources/MedalRepoPlugin/MedalRepoPlugin.cs) shows how they report game events to the recorder. This section of the code also tells us that they use [Harmony](https://github.com/pardeike/Harmony) (another standardized library) to patch their functions to the game. Turns out, they just use their [publicly accessible context and event api](https://docs.medal.tv/gameapi/index.html) to send data to the Medal app from the game, which makes it super easy for us to implement everything ourselves because we can request a public key of our own and use it. 

To find all the methods we can patch that the game uses we need to use something like [JetBrains DotPeek](https://www.jetbrains.com/decompiler/) to analyze the code for the game itself. This is complicated and varies game to game, best for anybody wanting to do so to look up a guide or message me directly and I can try to help out as best I can.

Anyways! I've added the following trackers that will record when some special moments happen in the game. This includes the player passing out for whatever reason, the player dying instantly (perhaps from a certain item or a friend getting a little too hungry?), for when a large/significant fall is detected (be warned, this one was tricky to figure out and may not be 100% accurate), and finally, for when scouts get too far from each other and meet the scoutmaster ;)

Sounds easy, but this took me way longer than I'd like to admit to implement; it's not exactly beginner friendly or open.

All the code for this is found inside [MedalPeakPlugin/src/MedalPeakPlugin/Plugin.cs](https://github.com/cosmaticdev/PEAK-ICYMI/blob/master/MedalPeakPlugin/src/MedalPeakPlugin/Plugin.cs)!

I've even gone ahead and nabbed a public api key for this project to use: pub_7yyLREtjlmTJeGtpI8wWR9NxpIkuvTF1