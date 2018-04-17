# Reading Data

One of the major benefits of using the ESRI platform it allows you to perform spatial and attribute based queries against the data to validate and perform analysis. As a side-effect, the same set of APIs are reused, which leads to code-duplication and/or memory management issues if used improperly.

## Fetch

The `ITable` and `IFeatureClass` interfaces have been extended to include `Fetch` methods that simplifies queries by abstracting the complexities while enforcing the proper memory management for the COM objects.

Method                        | Description
:-----------------------|:------------------------------|:----------------------|
Fetch(params int[])           | Fetch the features (or rows) that have the specified object identifiers using a non-recycling cursor.
Fetch(IQueryFilter)           | Fetch the features (or rows) that statisfy the query using a non-recycling cursor.
Fetch(IQueryFilter, Action<IRow>, bool)           | Fetch the features (or rows) that statisfy the query and execute the action each.

- `Fetch` features or rows with a list of OBJECTIDs.

    ```c#
    IEditor editor = ArcMap.Application.GetEditor();
    IWorkspace workspace = editor.EditWorkspace;

    IFeatureClass transformers = workspace.GetFeatureClass("Transformers");
    List<IFeature> features = transformers.Fetch(1,2,3,4,5,6);
    ```

- `Fetch` features or rows that need a spatial and/or attribute dependencies to filter the results.

    ```c#
    IEditor editor = ArcMap.Application.GetEditor();
    IWorkspace workspace = editor.EditWorkspace;

    IFeatureClass workRequests = workspace.GetFeatureClass("WorkRequests");
    IFeatureClass transformers = workspace.GetFeatureClass("Transformers");
    IFeature workRequest = workRequests.Fetch(1);
    ISpatialFilter filter = new SpatialFilterClass();
    filter.WhereClause = transformers.OIDFieldName + " IN (1,2,3,4,5,6)";
    filter.Geometry = workRequest.Shape;
    filter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
    filter.GeometryField = "SHAPE";

    List<IFeature> features = transformers.Fetch(filter);
    ```

- `Fetch` features or rows that need actions performed but the memory can be recycled.

    ```c#
    IEditor editor = ArcMap.Application.GetEditor();
    IWorkspace workspace = editor.EditWorkspace;

    IFeatureClass transformers = workspace.GetFeatureClass("Transformers");
    int i = transformers.FindField("DATEMODIFIED");

    IQueryFilter filter = new QueryFilterClass();
    filter.WhereClause = transformers.OIDFieldName + " IN (1,2,3,4,5,6)";

    int rowsAffected = transformers.Fetch(filter, feature =>
    {
        feature.set_Value(i, DateTime.Now);
        feature.Store();
    });
    ```

- `Fetch` features or rows with a projection.

    ```c#
    IEditor editor = ArcMap.Application.GetEditor();
    IWorkspace workspace = editor.EditWorkspace;

    IFeatureClass workRequests = workspace.GetFeatureClass("WorkRequests");
    IFeatureClass transformers = workspace.GetFeatureClass("Transformers");
    IFeature workRequest = workRequests.Fetch(1);
    ISpatialFilter filter = new SpatialFilterClass();
    filter.WhereClause = transformers.OIDFieldName + " IN (1,2,3,4,5,6)";
    filter.Geometry = workRequest.Shape;
    filter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
    filter.GeometryField = "SHAPE";

    List<int> oids = transformers.Fetch(filter, feature => feature.OID);
    ```
