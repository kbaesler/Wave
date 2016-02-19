# Developer Guide
This will serve as a reference guide for developer samples, the purpose of the samples is to provide example usages of the extensions that are provided in the **Wave Extensions for ArcGIS** and **Wave Extensions for ArcFM** packages.

- Not all of the features in the **Wave** project will be included in the samples. If you would like a sample of a feature please add a suggestion in the GitHub [issue tracker](https://github.com/Jumpercables/Wave/issues).

## Licenses
When a stand-alone executable needs to access and use geodatabase objects, a license must be checked out, depending on the product that has been installed.

### ArcGIS for Desktop
The following snippets shows the proper way to check out licenses when working with the ArcGIS for Desktop product.

```java
using(EsriRuntimeAuthorization lic = new EsriRuntimeAuthorization(ProductCode.Desktop))
{
    if(lic.Initialize(esriProductCode.esriProductCodeStandard))
    {
      Console.WriteLine("Success.")
    }
    else
    {
      Console.Writeline("Failed.")
    }
} // Check-in license
```

### ArcFM Solution
The following snippet shows the proper way to check out licenses when working with the ArcFM Solution and ArcGIS for Desktop products.

```java
using(RuntimeAuthorization lic = new RuntimeAuthorization(ProductCode.Desktop))
{
  if(lic.Initialize(esriProductCode.esriProductCodeStandard, mmLicensedProductCode.mmLPArcFM))
  {
    Console.WriteLine("Success.")
  }
  else
  {
    Console.Writeline("Failed.")
  }
} // Check-in licenses.
```

- When the ArcFM Solution has been installed and configured in the geodatabase, a license to both the ArcFM Solution and ArcGIS for Desktop is required.

## Geodatabase Connections
The `WorkspaceFactories` static class will return the proper workspace (`sde`, `gdb`, or `mdb`) based on the connection file parameter.

```java
var connectionFile = Path.Combine(Environment.GetFolderPath(SpecialFolders.ApplicationData), "\\ESRI\\Desktop\\ArCatalog\\Minerville.gdb")
var workspace = WorkspaceFactories.Open(connectionFile
var dbms = workspace.GetDBMS();
```

## Disabling Auto Updaters
There are cases when a **custom** ArcFM Auto Updater (AU) has been developed needs to temporarily `disable` subsequent calls.

```java
using (new AutoUpdaterModeReverter(mmAutoUpdaterMode.mmAUMNoEvents))
{
    // All of the ArcFM Auto Updaters are now disabled.
}
```

## Map

### Layers
The feature layers in the map can be traverse using the `Where` method extension.

```java
var layers = ArcMap.Application.GetActiveMap().Where(layer => layer.Valid);
var structures = layers.Where(layer => layer.FeatureClass.IsAssignedClassModelName("STRUCTURE"));
```

### Tables
The tables in the map can be traverse using the `Where` method extension.

```java
var tables = ArcMap.Application.GetActiveMap().GetTables(table => table.Valid);
var arcfm = tables.Where(table=> table.IsAssignedClassModelName("ARCFMSYSTEMTABLE"));
```

## Session / Workflow Manager
The Session Manager and Workflow Manager extensions to the ArcFM Solution are tightly coupled with the version management solution provided by the product.

### Session / Design Additions
When using Session Manager or Workflow Manager you often need to extend the ArcFM Solution Session or Design or Work Request to store client specific data, which can now be done by extending the `Session`, `Design` or `WorkRequest` classes.

```java
public class ClientSession : Session {

  public ClientSession(IMMPxApplication pxApp)
    : base(pxApp) {

  }

  public ClientSession(IMMPxApplication pxApp, int id)
    : base(pxApp, id) {

  }

  public string ClientName {get; set;}

  protected override bool Initialize(IMMSessionManager extension, int nodeID){

    if(!base.Initialize(extension, nodeID))
      return false;

    var tableName = base.Application.GetQualifiedTableName("CLIENT_SESSION");
    var commandText = string.Format("SELECT name FROM {0} WHERE session_id = {1}",
                                      tableName, nodeID);
    this.ClientName = base.Application.ExecuteScalar(commandText, "<null>");

    return true;
  }

  protected override bool Initialize(IMMSessionManager extension, IMMPxUser user) {

    if(!base.Initialize(extension, user))
      return;

    var tableName = base.Application.GetQualifiedTableName("CLIENT_SESSION");
    var commandText = string.Format("INSERT INTO {0} VALUES({1},'{2}')",
                                      tableName, base.ID, this.ClientName);
    int recordsAffected = base.Application.ExecuteNonQuery(commandText);
    return recordsAffected == 1;
  }
}
```

### Session / Design Accessors
When using Session Manager or Workflow Manager you will undoubtedly need to access the currently open session or design. The easiest way to access the data is through the `GetSession`, `GetWorkRequest` or `GetDesign` methods on the `IMMPxApplication` interface.

```java
var pxApplication = ArcMap.Application.GetPxApplication();
var session = pxApplication.GetSession();
var design = pxApplication.GetDesign();
var request = pxApplication.GetWorkRequest();
```

- If you have extended the `Session`, `WorkRequest` or `Design`, you can access it via the `action` parameter.

```java
var pxApplication = ArcMap.Application.GetPxApplication();
var session = pxApplication.GetSession((id, source) => new ClientSession(source, id))
```
