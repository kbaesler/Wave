# Wave

## Overview

[Wave](https://github.com/Jumpercables/Wave) is a C# library that extends the ArcGIS for Desktop and ArcFM Solution APIs in an effort to simplify customizing these products. The library has been designed to use extension methods to expose the features of the library and is built using [.NET Extension Methods](http://msdn.microsoft.com/en-us/library/bb383977.aspx) which allows for adding new features to existing interfaces and classes within the ArcGIS and ArcFM APIs. 

!!! info "Design"
	Any interfaces or objects that are extended have been setup to use the namespace of the object, which allows Wave's features to be available without adding new namespace delcarations.

	For instance, the `RowExtensions.cs` that contains extension methods for the `IRow` interface uses the `ESRI.ArcGIS.Geodatabase` namespace because that is the namespace that contains the `IRow` interface.

**Wave Extensions for ArcGIS** - The package for the ArcGIS for Desktop extensions.

[![Wave Extensions for ArcGIS](https://buildstats.info/nuget/Wave.Extensions.Esri)](https://www.nuget.org/packages/Wave.Extensions.Esri/)

**Wave Extensions for ArcFM** - The package for the ArcFM Solution extensions.

[![Wave Extensions for ArcFM](https://buildstats.info/nuget/Wave.Extensions.Miner)](https://www.nuget.org/packages/Wave.Extensions.Miner/)

## Documentation

You can [download](chms\Wave.v4.5.chm) the latest API documentation in CHM format.

!!! info "Download"
	On some systems, the contents of the CHM file is blocked. Right click on the CHM file, select **Properties**, and click on the **Unblock** button if it is present in the lower right corner of the **General** tab in the properties dialog.

## Installation

You can download a release of the library using the Microsoft NuGet Package Manager within Visual Studio.

To install **Wave Extensions for ArcGIS**, run the following command in the Package Manager Console.

```bat
PM> Install-Package Wave.Extensions.Esri
```

To install **Wave Extensions for ArcFM**, run the following command in the Package Manager Console.

```bat
PM> Install-Package Wave.Extensions.Miner
```

!!! info "Dependencies"
	The **Wave.Extensions.Esri** package features are included in the **Wave.Extensions.Miner** package, as the *ArcFM Solution* product is tightly coupled with the *ArcGIS for Desktop* product.

### Requirements

- ArcGIS for Desktop 10 (or higher)
- ArcFM Solution 10 (or higher)
- 4.5 .NET Framework
- Visual Studio 2015 (or higher)

### Third Party Libraries

- log4net 2.0.3 (or higher)
