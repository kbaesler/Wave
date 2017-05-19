# Reading Data
One of the major benefits of using the ESRI platform it allows you to perform spatial and attribute based queries against the data to validate and perform analysis. As a side-effect, the same set of APIs are reused, which leads to code-duplication and/or memory management issues if used improperly.

## Fetch
The `ITable` and `IFeatureClass` interfaces have been extended to include `Fetch` methods that simplifies queries by abstracting the complexities while enforcing the proper memory management for the COM objects.

- `Fetch` features or rows with a list of OBJECTIDs.

```c#
    List<IFeature> features = featureClass.Fetch(1,2,3,4,5,6);
```

- `Fetch` features or rows that need a spatial and/or attribute dependencies to filter the results.

```c#  
ISpatialFilter filter = new SpatialFilterClass();
filter.WhereClause = featureClass.OIDFieldName + " IN (1,2,3,4,5,6)";
filter.Geometry = geometry;
filter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
filter.GeometryField = "SHAPE";

List<IFeature> features = featureClass.Fetch(filter);
```

- `Fetch` features or rows that need actions performed but the memory can be recycled.

```c#
int i = featureClass.FindField("DATE");

IQueryFilter filter = new QueryFilterClass();
filter.WhereClause = featureClass.OIDFieldName + " IN (1,2,3,4,5,6)";

int rowsAffected = featureClass.Fetch(filter, feature =>
{
     feature.set_Value(i, DateTime.Now);
     feature.Store();
});
```

- `Fetch` features or rows with a projection.

```c#
ISpatialFilter filter = new SpatialFilterClass();
filter.WhereClause = featureClass.OIDFieldName + " IN (1,2,3,4,5,6)";
filter.Geometry = geometry;
filter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
filter.GeometryField = "SHAPE";

List<int> oids = featureClass.Fetch(filter, feature => feature.OID);
```
