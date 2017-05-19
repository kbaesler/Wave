# Component Registration
The **Wave Extensions for ArcGIS** and **Wave Extensions for ArcFM** packages will automatically import the `Wave.Extensions.Esri.targets` and `Wave.Extensions.Miner.targets` into the Visual Studio Project File when the packages are installed. These `MS Build` target files are used to register the `ESRI` and `ArcFM` components when the project is compiled.

- `Wave.Extensions.Esri.targets`
```xml
<?xml version="1.0" encoding="utf-8"?>
<Project xmlns="http://schemas.microsoft.com/developer/msbuild/2003">  
  <PropertyGroup>
    <RegisterForComInterop Condition="'$(RegisterForComInterop)' == ''">False</RegisterForComInterop>
  </PropertyGroup>
  <Target Name="BeforeClean" Condition="$(RegisterForComInterop) == 'True'"> 	
	<Message Text="Wave.Extensions.Esri.targets: Unregistered"/>    
	<Exec WorkingDirectory="$(CommonProgramFiles)\ArcGIS\bin" Command="ESRIRegAsm.exe &quot;$(TargetPath)&quot; /p:Desktop /u /s" Condition="Exists('$(TargetPath)')" ContinueOnError="true"/>  		
  </Target>
  <Target Name="AfterBuild" Condition="$(RegisterForComInterop) == 'True'">  
	<Message Text="Wave.Extensions.Esri.targets Registered"/>  
	<Exec WorkingDirectory="$(CommonProgramFiles)\ArcGIS\bin" Command="ESRIRegAsm.exe &quot;$(TargetPath)&quot; /p:Desktop /s" ContinueOnError="true" />		
  </Target>
</Project>
```

- `Wave.Extensions.Miner.targets`
```xml
<?xml version="1.0" encoding="utf-8"?>
<Project xmlns="http://schemas.microsoft.com/developer/msbuild/2003">  
  <PropertyGroup>
    <RegisterForComInterop Condition="'$(RegisterForComInterop)' == ''">False</RegisterForComInterop>
  </PropertyGroup>
  <Target Name="BeforeClean" Condition="$(RegisterForComInterop) == 'True'"> 	
	<Message Text="Wave.Extensions.Miner.targets: Unregistered"/>    
	<Exec WorkingDirectory="$(CommonProgramFiles)\ArcGIS\bin" Command="ESRIRegAsm.exe &quot;$(TargetPath)&quot; /p:Desktop /u /s" Condition="Exists('$(TargetPath)')" ContinueOnError="true"/>  	
	<Exec WorkingDirectory="$(ProgramFiles)\Miner and Miner\ArcFM Solution\Bin" Command="RegX.exe &quot;$(TargetPath)&quot; /d /u" Condition="Exists('$(TargetPath)')" ContinueOnError="true" />		
  </Target>
  <Target Name="AfterBuild" Condition="$(RegisterForComInterop) == 'True'">  
	<Message Text="Wave.Extensions.Miner.targets Registered"/>  
	<Exec WorkingDirectory="$(CommonProgramFiles)\ArcGIS\bin" Command="ESRIRegAsm.exe &quot;$(TargetPath)&quot; /p:Desktop /s" ContinueOnError="true" />
	<Exec WorkingDirectory="$(ProgramFiles)\Miner and Miner\ArcFM Solution\Bin" Command="RegX.exe &quot;$(TargetPath)&quot; /d" ContinueOnError="true" />		
  </Target>
</Project>
```