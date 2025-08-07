<!-- If you plan to build this project from source please remove the .md extension from this file.-->
<!-- If your build has issues finding the PEAK libraries fill in the variables listed bellow, otherwise leave them commented. -->

<Project>
  <PropertyGroup>
    <!-- Set if the default steam game install directory doesn't work. -->
    <!--<PeakGameRootDir></PeakGameRootDir>-->
    <!-- Set if your BepInEx/plugins/ directory isn't in the game files. -->
    <!-- <PeakPluginsDir></PeakPluginsDir> -->
  </PropertyGroup>

  <Target Name="DeployFiles" AfterTargets="Build">
    <Message Text="Deploy → $(PeakPluginsDir)$(AssemblyName).dll" Importance="High" />
    <Error
      Text="Plugins directory '$([MSBuild]::NormalizeDirectory($(PeakPluginsDir)))' doesn't exist! Configure PeakPluginsDir to point to a valid path in the Config.Build.user.props file."
      Condition="!Exists('$(PeakPluginsDir)')"
    />
    <Copy SourceFiles="$(TargetPath)" DestinationFolder="$(PeakPluginsDir)" />
  </Target>
</Project>
