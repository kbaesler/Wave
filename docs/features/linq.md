# LINQ
The `while` statement is used in conjunction with the ArcFM and ArcGIS APIs to iterate through collections using the `Reset()` and `Next()` method combinations. However, in most cases the `foreach` statement is the preferred method, thus the most frequently used iterators can be converted to an enumerable type using the `AsEnumerable()` extension method, which allows these collections to now take advantage of `LINQ`.

The following is a short list of the interfaces that support enumerable types:

 ESRI                          | ArcFM                         
:------------------------------ |:------------------------------
 `ILongArray`                | `IMMEnumField`              
 `IArray`                    | `IMMEnumTable`              
 `IEnumDomain`               | `IMMEnumObjectClass`        
 `IEnumBSTR`                 | `IMMEnumFeederSource`       
 `IEnumIDs`                  | `IMMPxNodeHistory`          
 `IFields`                   | `IMMEnumPxTransition`       
 `IEnumRelationshipClass`    | `IMMPxNode`                 
 `IEnumFeatureClass`         | `IMMEnumPxState`            
 `ICursor`                   | `IMMEnumPxTasks`            
 `IFeatureCursor`            | `IMMEnumPxUser`             
 `ISet`                      | `IMMEnumPxFilter`           
 `...`                         | *There are many more that haven't been listed for the sake of brevity.*

These new extension methods allow you to write code like this:

```java
int count = cursor.AsEnumerable().Count();
```

and:

```java
var results =
    from x in cursor1.AsEnumerable()
    from y in cursor2.AsEnumerable()
    where x.get_Value(0) = y.get_Value(0)
    select new { Left = x, Right = y };
```

There are several objects (i.e. `IEnumLayer`, `IMap`, `ID8List`) in ArcFM and ArcGIS APIs that require recursion to obtain all of the data. In the past, you'd have to create a recursive method for iterating the contents of these hierarchical structures. These structures can now be traversed recursively using `LINQ`.

- Traversing the contents of the `IMap`.

```c#
public void ClearDefinitions(IMap map)
{
    var layerDefinitions = map.Where(o => o.Visible).Select(o => (IFeatureLayerDefinition2) o)
    foreach (var layerDefinition in layerDefinitions)
    {
        layerDefinition.DefinitionExpression = null;
    }
}
```
- Traversing the contents of the `ID8List`.

```c#
public void UpdateTags(ID8List list, string tag)
{
    var items = list.Where(o => o is IMMProposedObject).Select(o => (IMMProposedObject) o.Value)
    foreach (var item in items)
    {
        IMMFieldManager fieldManager = item.FieldManager;
        IMMFieldAdapter fieldAdapter = fieldManager.FieldByName("TAG");
        fieldAdapter.Value = tag;
        item.Update(null);
    }
}
```