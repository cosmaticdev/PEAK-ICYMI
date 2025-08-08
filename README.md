<h1>
In Case You Missed It; Automatic Medal clipping for PEAK<br>
<span style="font-size:0.6em;">v1.0 8/8/2025 by cosmatic</span>
</h1>

<h2>What events are detected?</h2>
<ul>
    <li>Whenever your player passes out</li>
    <li>Using an item that kills you instantly</li>
    <li>Suffering from a major fall</li>
    <li>Getting attacked by the Scoutmaster...</li>
</ul>
All events have a few seconds of delay before clipping to help capture reactions or results of whatever happened!
<br><br>
Overlapping events? What if I fall and get knocked out from it, do I get duplicate clips?<br>
Nope! There's cooldowns between events which make sure that events don't capture tons of duplicate footage.

<h2>Contextualization</h2>
By default this plugin will send lists of players in your PEAK lobby to Medal so that they can detect who you are playing with during your clips.
This helps Medal suggest players you've played with as Medal followers and to recommend for tags in clips.

<h2>Installation<br>
<span style="font-size:0.6em;">There are three primary ways to install this plugin, they are listed bellow.</span>
</h2>
<h3>1.) ZIP Extraction (recommended)</h3>
If you have installed a mod directly to PEAK before (thunderstore or another mod manager don't count) I would recommend using method 2
<ul>
    <li>navigate to <a url="https://github.com/cosmaticdev/PEAK-ICYMI/releases" target="_blank">github.com/cosmaticdev/PEAK-ICYMI/releases</a> and install the zip folder for the latest release</li>
    <li>navigate to your PEAK directory (typically C:\Program Files (x86)\Steam\steamapps\common\PEAK or [external drive]:\SteamLibrary\steamapps\common\PEAK)</li>
    <li>extract the ZIP folder in to the root of the game's directory</li>
    <li>boot PEAK and enjoy.</li>
</ul>
<h3>2.) DLL Installation (if you've used BepInEx with PEAK before)</h3>
<ul>
    <li>navigate to <a url="https://github.com/cosmaticdev/PEAK-ICYMI/releases" target="_blank">github.com/cosmaticdev/PEAK-ICYMI/releases</a> and install MedalPeakPlugin.dll</li>
    <li>navigate to your PEAK directory (typically C:\Program Files (x86)\Steam\steamapps\common\PEAK or [external drive]:\SteamLibrary\steamapps\common\PEAK)</li>
    <li>go to your BepInEx plugin subfolder (normally BepInEx\plugins)</li>
    <li>place the MedalPeakPlugin.dll DLL in to the plugin folder</li>
    <li>boot the game and have fun</li>
</ul>
<h3>3.) Building For Yourself (must be technical, C# & .NET experience recommended)</h3>
<ul>
    <li>clone this repository wherever you'd like</li>
    <li>rename PEAK-ICYMI\MedalPeakPlugin\Config.Build.user.props.md to PEAK-ICYMI\MedalPeakPlugin\Config.Build.user.props (remove the .md)</li>
    <li>open the file in your editor and read the comments at the top of the file, and follow what they say (this is a fallback file, basically)</li>
    <li>open the \MedalPeakPlugin in your terminal</li>
    <li>you probably already have .NET but if you don't install it <a url="https://dotnet.microsoft.com/en-us/download" target="_blank">here, from Microsoft</a></li>
    <li>run dotnet build and wait for compilation to be finished. If your game paths are correct the DLL should be added to the game directory automatically</li>
    <li>you can always double check it was added by going to PEAKS \BepInEx\plugins folder and searching for it</li>
    <li>if the build compiled correctly you should be able to boot the game and be good to go! If you plan to edit the plugin check out technical.md to see how I've done it</li>
</ul>

<h2>How was this done? Why?</h2>
This project was made by building off of Medal's REPO plugin that functions quite similar to what I've done here. The full technical details of my research in to learning Medal's implimentation is able to be found at <a url="https://github.com/cosmaticdev/PEAK-ICYMI/blob/master/technical.md" target="_self">PEAK-ICYMI/technical.md</a> but it also took me a few dozen hours of learning BepInEx to leverage the methods PEAK has internally aswell as figuring out C# to bring the project to life (C# is actually incredibly easy to read, so even if you've never used it I'd totally recommend giving BepInEx development a shot if you have experience with Java, JavaScript, or anything similar). If you plan to do something similar then patching methods is probably going to be your biggest headache, but theres loads of community discords if you ever need help.
Oh yeah, and for my reasons why! I've been using Medal for a few years now and really love the app and community, and just wanted to make a little something that might help make it a little better. Also this was a fun opportunity to learn about Unity and game modding ;)

<h2>Notes</h2>
This project would not be possible without <a url="https://github.com/BepInEx/BepInEx" target="_blank">BepInEx</a> (a Unity modding framework), the <a url="https://github.com/PEAKModding/BepInExTemplate/tree/main" target="_blank">PEAKModding BepInEx template</a>, the <a url="https://discord.com/invite/peak-modding-community-1363179626435707082" target="_blank">Peak Modding Community</a> on Discord, and the reference code (found in /resources) I found from the team at <a url="https://medal.tv" target="_blank">Medal.TV</a>, so thank you to all of them.

<br>If you would like to see my preperation and planning for this project, please read <a url="https://github.com/cosmaticdev/PEAK-ICYMI/blob/master/technical.md" target="_self">PEAK-ICYMI/technical.md</a>.
<br>And of course, if you would like to contact me for any reason at all, I am available nearly every day on discord <a url="https://discord.com/users/646450857801547799" target="_blank">@cosmatic_</a>