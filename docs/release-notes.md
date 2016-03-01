# Release Notes
This will serve as a reference to the release notes with regards to the `release` version of the NuGet packages that are published.

## Roadmap
These are the changes, additions, removals that are actively being worked on and will be included in future releases.

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
- The  `ReferenceDictionary` has been replaced with the `ToDictionary` extension method on the `Miner.Interop.Process.IDictionary` which converts it to `Dictionary{String, Object}`. [^2] [^4]

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


 [^1]: A change that is included in the **Wave Extensions for ArcGIS** package.

 [^2]: A change that is included in the **Wave Extensions for ArcFM** package.

 [^3]: A change that is included in both the **Wave Extensions for ArcGIS** and **Wave Extensions for ArcFM** packages.

 [^4]: A change that *potentially* causes other components to **fail**.
