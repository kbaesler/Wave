# Release Notes
This will serve as a reference to the release notes with regards to the `release` version of the nuget packages that are published.

## Version 1.0.1 - *Coming Soon*
### Added
- New extensions for the `ISqlWorkspace` interface. [^1]
- Added `GetWorkspaces` to the `IApplication` interface which will return the `IMMStandardWorkspace` interface that will give the `LoginWorkspace`, `LibraryWorkspace`, and `EditWorkspace`. [^2]
- Added getter property for the `Locked` property on the `IPxControlUI` interface. [^2]
- Added `GetCULibrary` to the `IApplication` interface which will return the `ICuLibrary` interface used for interacting with the storage of the library.

### Removed
- The `CreateNew` and `Initialize` methods on the `IPxNode` interface have been removed. [^2]

### Changed
- Using overloading mechanism instead of the optional parameters. [^3]
- The `Session`, `Design` and `WorkRequest` classes updated to reflect changes to the `IPxNode` interface and now create and initialize based on the constructor parameters. [^2]
- Instance members should not write to `static` fields in `ComboTreeDropDown.cs`. [^1]
- Removed `virtual` from the `DictionaryChanged` and `DictionaryChanging` events in the `ObservableDictionary.cs` [^1]
- Initialize the `BehaviorsProperty` property in static constructor in the `CommandBehaviorCollection.cs` [^1]
- Defined the locale to be used in `String` operations. [^3]

---

## Version 1.0.0 - 2016-02-06

### New
 - Initial support for version 10 of the ArcFM Solution and ArcGIS for Desktop products

 [^1]: **Wave Extensions for ArcGIS** package.

 [^2]: **Wave Extensions for ArcFM** package.

 [^3]: **Wave Extensions for ArcGIS** and **Wave Extensions for ArcFM** packages.
