# Developer Samples
This will serve as a simple reference guide for developer samples, the purpose of the samples is to provide simple cases of usages of the extensions that are provided in the `Wave Extensions for ArcGIS` and `Wave Extensions for ArcFM` packages.

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
}
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
}
```
> **Note**
When the ArcFM Solution has been installed and configured in the geodatabase, a license to both the ArcFM Solution and ArcGIS for Desktop is required.

## Connecting to Geodatabase
The `WorkspaceFactories` static class will return the proper workspace (`sde`, `gdb`, or `mdb`) based on the connection file parameter.

```java
var connectionFile = Path.Combine(Environment.GetFolderPath(SpecialFolders.ApplicationData), "\\ESRI\\Desktop\\ArCatalog\\Minerville.gdb")
var workspace = WorkspaceFactories.Open(connectionFile)
```
## Disabling Auto Updaters
There are cases when a **custom** ArcFM Auto Updater (AU) has been developed needs to temporarily `disable` subsequent calls to the AU, because internally it needs to update features for subsequent objects for the same table or feature class that has been assigned the AU.

```java
using (new AutoUpdaterModeReverter(mmAutoUpdaterMode.mmAUMNoEvents))
{
    // All of the ArcFM Auto Updaters are now disabled.
}
```

> When this situation is encountered, if the AU does not `disable` the ArcFM Auto Updater Framework, an **infinite** loop will occur when AU is executed.
