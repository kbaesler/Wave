# Frequently Asked Questions
This will serve as a list of all of the frequently ask questions.

If you don't see your question answered here, please submit an issue on the [GitHub](https://github.com/Jumpercables/Wave/issues) page with your question.

## 1. What is Wave?
Wave is meant to provide a simplistic approach to extending the ArcGIS and ArcFM platforms by providing simplified implementations for complex operations, frequently use call stacks, enforcing the recommended best practices and create a seamless integration.

## 2. What was the motivation?
Over the years as a developer in the Geographic Information System (GIS) for Utilities industry there's been an increasing need for a way to make development easier as the industries needs increase in complexity.

## 3. What is ArcGIS?
[ArcGIS](http://resources.arcgis.com/en/help/getting-started/articles/026n00000014000000.htm) is a comprehensive system that allows people to collect, organize, manage, analyze, communicate, and distribute geographic information. As the world's leading platform for building and using geographic information systems (GIS), ArcGIS is used by people all over the world to put geographic knowledge to work in government, business, science, education, and media. ArcGIS enables geographic information to be published so it can be accessed and used by anyone. The system is available everywhere using web browsers, mobile devices such as smartphones, and desktop computers.

## 4. What is ArcFM?
[ArcFM](http://www.schneider-electric.com/solutions/ww/en/sol/26048721-geospatial-intelligence--arcfm-solution?other=-1) is an Enterprise GIS with reliable network intelligence that provides information such as pipe water direction flows, the number of customers attached to a gas meter, and what devices are downstream of a particular fuse.

## 5. Do I need both ArcFM and ArcGIS to use Wave?
Wave has been developed with it's dependencies in mind to allow the library to be used in two different ways:

1. When only the **ArcGIS for Desktop** is installed, you'll want to install the [Wave Extensions for ArcGIS](https://www.nuget.org/packages/Wave.Extensions.Esri/) package.

2. When both the **ArcGIS for Desktop** and **ArcFM Solution**, you'll want to install the [Wave Extensions for ArcFM](https://www.nuget.org/packages/Wave.Extensions.Miner/)  package.

## 6. Do I need a license to use Wave?
Wave is open-source and doesn't directly require any licenses, however since both **ArcFM** and **ArcGIS** are commercial software packages that require licenses.

## 8. Where are the packages published?
The packages are published to the [NuGet](https://www.nuget.org) (provided by Microsoft) service to allow the community to consume the libraries without needing to download and compile the source.

## 9. What's installed with the packages?
When a package is being “installed” it will be downloaded to the local machine and will modify the project in the following ways.

1.	Three files will be added to the project.
    -	`LogInfo.cs` – Contains the necessary information for using log4net configurations.
    -	`Wave.log4net.config` – Contains the default configurations used for logging information.
    -	`packages.config` – Contains information about the packages that have been installed.

2.	A packages folder will be added to the root of the solution. This folder will contain the contents of the packages that have been installed.

3.	The `*.csproj` file will be modified to include the MSBuild targets (that are included in the packages), that will automate the ESRI and MINER component registration.
    -	**Must** be using the Attribute declarations on the classes.
    -	**Must** have the Register for COM option checked within Visual Studios.

4. A reference to the `log4net` assembly will be downloaded and added to the project (as it is a dependency of the Wave packages).

## 10. What logging framework is used by Wave?
We have elected to use `log4net` as the logging framework.

## 11. What `log4net` configuration file is used?
The `Wave.Extensions.Esri` and `Wave.Extensions.Miner` package have been configured to use the configuration file named `Wave.log4net.config` (which is installed with the packages).

  * The configuration file must reside in the directory of the running application. For example, `%PROGRAMFILES%\ArcGIS\Bin` and `%PROGRAMFILES%\Miner and Miner\ArcFM Solution\Bin` when the assemblies are used for extensions within ArcGIS for Desktop or ArcFM Solution.

## 12. How are the packages distributed?
The packages are distributed using [NuGet](https://www.nuget.org) (provided by Microsoft), which is a Visual Studio extension that makes it easy to add, remove and update libraries and tools in Visual Studio projects that use the .NET Framework.
