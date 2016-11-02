# Release Notes
This will serve as a reference to the release notes with regards to the `release` version of the NuGet packages that are published.

## Backlog
These are the changes, additions, removals that are actively being worked on and will be included in future releases.

### New
- The `Wave.log4net.config` file is included in the packages. [^3]
- Added `Version` and `BuildNumber` to the `ArcFM` static class. [^2]
- Added `GetVisibleLayers` extension method to the `IMap` interface which will return only those layers that are visible (either by virtue of being enabled or due to scale suppression). [^1]
- Added *Linear Referencing* support classes in the `ESRI.ArcGIS.Location` namespace. [^1]
- Added `Export` extension methods to the `IFeatureClass` and `ITable` interface which allows for exporting the data to another data source. [^1]
- Added `Delete` extension methods to the `IFeatureClass` and `ITable` interface for deleting the object from the database. [^1]
- Added support for converting `Dictionary{string, TValue}` into a `dynamic` dictionary using the `ToDynamic` extension method off the `Dictionary{string, TValue}` class. [^1] [^5]
- Added support for converting `XDocument` into a `dynamic` dictionary using the `ToDynamic` extension method off the `XDocument` class. [^1] [^5]

## Fixed
- Fixed issue with `GetAutoValue` extension methods were recursively searching too deep for the auto values.
- Fixed issue with `ReadCsv` extension method for the `DataTable` that wasn't opening the ADO connection prior to executing the read, which was causing an exception.

!!! warning "Unpublished"
    The changes, additions, removals and new features that are part of the backlog have not been published to the public as nuget packages.

---
## Version 1.0.3 - 2016-04-04
### New
- Added `PerformOperation` extension methods to the `IEditor`, `IMMEditor`, `IWorkspace` and `IWorkspaceEdit` interfaces that wrap the start / stop operation constructs. [^3]
- Added `ExecuteTask` extension method on the `IMMPxNode` object to execute tasks based on name. [^2]
- Added `CompatibleUnit` and `WorkLocation` node objects. [^2]
- Added `GetActiveTab` extension method to the `IMMAttributeEditor` interface to allow for returning the `ID8List` for the tab that is selected in the ArcFM Attribute Editor. [^2]
- Added `Pan`, `Zoom`, and `Flash` extension methods to the `IFeature` interface to allow for performing these actions when the proper hook is supplied. [^1]
- Added `Unhighlight` extension method for unhighlighting features. [^2]

### Changed
- The `GetDataChanges` extension methods on the `IVersion` interface returns a `DeltaRowCollection` instead of a `List{DeltaRow}` as the accessor methods on the `DeltaRow` have been moved to the `DeltaRowCollection` class to allow for better performance. [^1] [^4]
- The `ESRI.ArcGIS.Framework.BaseClasses` namespace was replaced with the `ESRI.ArcGIS.ADF.BaseClasses` namespace. [^1] [^4]
- The `Miner.Framework.BaseClasses` namespace was replaced with the `Miner.Interop` namespace. [^2] [^4]
- Moved the `IProgressBarAnimation` interface into the `ESRI.ArcGIS.Framework` namespace [^1]

---
## Version 1.0.2 - 2016-03-03
### Fixed
- Due to an issue with the build server the 4.5 .NET Framework packages were not included.

---
## Version 1.0.1 - 2016-03-01
### New
- The packages now support the 3.5 and 4.5 .NET Framework. [^3]
- The 4.5 .NET Framework packages now include supporting `async` methods. [^3]

### Added
- Added `Execute` extension method to the `ISqlWorkspace` interface to support query cursors. [^1]
- Added `GetWorkspaces` extension method to the `IApplication` interface which will return the `IMMStandardWorkspace` interface that will give the `LoginWorkspace`, `LibraryWorkspace`, and `EditWorkspace`. [^2]
- Added getter property for the `Locked` property on the `IPxControlUI` interface. [^2]
- Added `GetCULibrary` to the `IApplication` interface which will return the `ICuLibrary` interface used for interacting with the storage of the library. [^2]
- Added `GetHistory` extension methods to the `IMMPxApplication` for retrieving historical data for nodes. [^2]
- Added `GetNodeTypeName` extension method to the `IMMPxApplication` to reduce the number of casts necessary to retrieve the information from the node objects. [^2]


### Removed
- The `CreateNew` and `Initialize` methods on the `IPxNode` interface have been removed. [^2] [^4]
- The `ReferenceDictionary` has been replaced with the `ToDictionary` extension method on the `Miner.Interop.Process.IDictionary` which converts it to `Dictionary{String, Object}`. [^2] [^4]

### Changed
- Using overloading mechanism instead of the optional parameters. [^3]
- The `Session`, `Design` and `WorkRequest` classes updated to reflect changes to the `IPxNode` interface and now create and initialize based on the constructor parameters. [^2]
- Instance members should not write to `static` fields in `ComboTreeDropDown`. [^1]
- Removed `virtual` from the `DictionaryChanged` and `DictionaryChanging` events in the `ObservableDictionary` [^1]
- Initialize the `BehaviorsProperty` property in static constructor in the `CommandBehaviorCollection` [^1]
- Defined the locale to be used in `String` operations. [^3]
- The `StartEditing` extension methods on the `IVersion` and `IWorkspace` interfaces were removed, as they were duplicates of the `PerformOperation` methods.[^1] [^4]
- The `BaseAutoText` implementation no longer requires the `progId` parameter in the constructor as it is derived from the `ProgIdAttribute` that is assigned to the class. [^2] [^4]
- A `Design` can no longer be created as an orphan, it requires a `WorkRequest` or `IMMWMSWorkRequest` to be constructed.
---

## Version 1.0.0 - 2016-02-06

### New
 - Initial support for version 10 of the ArcFM Solution and ArcGIS for Desktop products


!!! note "Releases"
    You can install and use a previous release of the packages by specifying the package version in the Package Manager Console window.

   [^1]: A change that is included in the **Wave Extensions for ArcGIS** package.

   [^2]: A change that is included in the **Wave Extensions for ArcFM** package.

   [^3]: A change that is included in both the **Wave Extensions for ArcGIS** and **Wave Extensions for ArcFM** packages.

   [^4]: A change that **potentially** causes other components to **fail**.

   [^5]: A change that requires the **.NET 4.5** framework.
