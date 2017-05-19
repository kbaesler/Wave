# Editing Data
There is a lot of editing that takes place within the ESRI platform. You can easily wrap up the necessary stop / start operations using the different flavors of the `PerformOperation` extension method on the `IWorkspace`, `IWorkspaceEdit`, `IVersion`, and `IEditor` interfaces.

## Nonversioned Edits
There are situations when you need to update only non-versioned tables inside the GIS. You can use the following method, and specific the proper enumeration to ensure that you are only editing the non-versioned tables inside the workspace.

```c#
// Update the description of Denver, in the City table that is a non-versioned table.
workspace.PerformOperation(true, esriMultiuserEditSessionMode.esriMESMNonVersioned,
   ()=>
   {
       var cities = workspace.GetTable(typeof(City));
       var filter = new QueryFilter();
       filter.Where = "City = 'Denver'";

        var denver = cities.Map<City>(filter).SingleOrDefault();
        denver.Description = "The capital of Colorado.";
        denver.Update();

        return true; // Commit the operation.
    });
```

## Versioned Edits
When editing a version table or feature class, you can take the same actions as above, except specify the versioned enumeration.

```c#
// Update the Begin and End stationing values for the Route with the ID. 
workspace.PerformOperation(true, esriMultiuserEditSessionMode.esriMESMVersioned,
   ()=>
   {
       var route = workspace.GetFeatureClass(typeof(RouteCenterline));
       var filter = new QueryFilter();
       filter.Where = "ROUTEID = 10423";

        var centerline = route.Map<RouteCenterline>(filter).SingleOrDefault();
        centerline.Begin = 0;
        centerline.End = 100;
        centerline.Update();

        return true; // Commit the operation.
    });
```
