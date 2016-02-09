
# Wave #
[![Documentation Status](https://readthedocs.org/projects/wave/badge/?version=latest)](http://wave.readthedocs.org/en/latest/)

Wave is C# library extends the ArcGIS for Desktop and ArcFM Solution APIs in an effort to simplify customizing these products. The library has been designed to use extension methods to expose the features of the library.

## Documentation ##
The latest documentation is at [wave.readthedocs.org](http://wave.readthedocs.org).

## NuGet ##
You can download a release of the library using the Microsoft NuGet Package Manager within Visual Studio.

To install **Wave Extensions for ArcGIS**, run the following command in the Package Manager Console.

```
	PM> Install-Package Wave.Extensions.Esri
```

To install **Wave Extensions for ArcFM**, run the following command in the Package Manager Console.

```
	PM> Install-Package Wave.Extensions.Miner
```

> The **Wave.Extensions.Esri** package features are included in the **Wave.Extensions.Miner** package, as the *ArcFM Solution* product is tightly coupled with the *ArcGIS for Desktop* product.

### Requirements ###
- ArcGIS for Desktop 10 (or higher)
- ArcFM Solution 10 (or higher)
- 3.5 SP 1 .NET Framework
- Visual Studio 2010 (or higher)

### Third Party Libraries ###
- log4net 2.0.3
