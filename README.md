## ICYMI; Automatic Medal clipping for PEAK<br>v1.0 [coming soon] by cosmatic
Automatically clip epic moments in PEAK using Medal's public [game API](https://docs.medal.tv/gameapi/index.html). Medal must be installed and running for this plugin to have any functionality.

## What events are detected?
- Whenever your player passes out
- Using an item that kills you instantly
- Suffering from a major fall
- Getting attacked by the Scoutmaster...

All events have a few seconds of delay before clipping to help capture reactions or results of whatever happened!

Overlapping events? What if I fall and get knocked out from it, do I get duplicate clips?<br>
Nope! There's cooldowns between events which make sure that events don't capture tons of duplicate footage.

## Contextualization
By default this plugin will send lists of players in your PEAK lobby to Medal so that they can detect who you are playing with during your clips.
This helps Medal suggest players you've played with as Medal followers and to recommend for tags in clips.

## Installation
There are three primary ways to install this plugin, they are listed bellow.

### 1.) ZIP Extraction (recommended)
If you have installed a mod directly to PEAK before (thunderstore or another mod manager don't count) I would recommend using method 2
- navigate to [github.com/cosmaticdev/PEAK-ICYMI/releases](https://github.com/cosmaticdev/PEAK-ICYMI/releases) and install the zip folder for the latest release
- navigate to your PEAK directory (typically C:\Program Files (x86)\Steam\steamapps\common\PEAK or [external drive]:\SteamLibrary\steamapps\common\PEAK)
- extract the ZIP folder in to the root of the game's directory
- boot PEAK and enjoy.

### 2.) DLL Installation (if you've used BepInEx with PEAK before)
- navigate to [github.com/cosmaticdev/PEAK-ICYMI/releases](https://github.com/cosmaticdev/PEAK-ICYMI/releases) and install MedalPeakPlugin.dll
- navigate to your PEAK directory (typically C:\Program Files (x86)\Steam\steamapps\common\PEAK or [external drive]:\SteamLibrary\steamapps\common\PEAK)
- go to your BepInEx plugin subfolder (normally BepInEx\plugins)
- place the MedalPeakPlugin.dll DLL in to the plugin folder
- boot the game and have fun

### 3.) Building For Yourself (must be technical, C# & .NET experience recommended)
- clone this repository wherever you'd like
- rename PEAK-ICYMI\MedalPeakPlugin\Config.Build.user.props.md to PEAK-ICYMI\MedalPeakPlugin\Config.Build.user.props (remove the .md)
- open the file in your editor and read the comments at the top of the file, and follow what they say (this is a fallback file, basically)
- open the \MedalPeakPlugin in your terminal
- you probably already have .NET but if you don't install it [here, from Microsoft](https://dotnet.microsoft.com/en-us/download)
- run ```dotnet build``` in the terminal and wait for compilation to be finished. If your game paths are correct the DLL should be added to the game directory automatically
- you can always double check it was added by going to PEAKS \BepInEx\plugins folder and searching for it
- if the build compiled correctly you should be able to boot the game and be good to go! If you plan to edit the plugin check out technical.md to see how I've done it

## How was this done? Why?
This project was made by building off of Medal's REPO plugin that functions quite similar to what I've done here. The full technical details of my research in to learning Medal's implimentation is able to be found at [PEAK-ICYMI/technical.md](https://github.com/cosmaticdev/PEAK-ICYMI/blob/master/technical.md) but it also took me a few dozen hours of learning BepInEx to leverage the methods PEAK has internally aswell as figuring out C# to bring the project to life (C# is actually incredibly easy to read, so even if you've never used it I'd totally recommend giving BepInEx development a shot if you have experience with Java, JavaScript, or anything similar). If you plan to do something similar then patching methods is probably going to be your biggest headache, but theres loads of community discords if you ever need help.
Oh yeah, and for my reasons why! I've been using Medal for a few years now and really love the app and community, and just wanted to make a little something that might help make it a little better. Also this was a fun opportunity to learn about Unity and game modding ;)

## Notes
This project would not be possible without [BepInEx](https://github.com/BepInEx/BepInEx) (a Unity modding framework), the [PEAKModding BepInEx template](https://github.com/PEAKModding/BepInExTemplate/tree/main), the [Peak Modding Community](https://discord.com/invite/peak-modding-community-1363179626435707082) on Discord, and the reference code (in [/resources](https://github.com/cosmaticdev/PEAK-ICYMI/tree/master/resources)) I found from the team at [Medal.TV](https://medal.tv), so thank you to all of them.

If you would like to see my preperation and planning for this project, please read [PEAK-ICYMI/technical.md](https://github.com/cosmaticdev/PEAK-ICYMI/blob/master/technical.md).

And of course, if you would like to contact me for any reason at all, I am available nearly every day on discord [@cosmatic_](https://discord.com/users/646450857801547799)