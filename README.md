# Wave #

Wave is a C# library that extends the ESRI ArcObjects and Schneider Electric ArcFM APIs by developing classes as extension methods whenever possible. This design concept eliminates the need for the developer to learn new namespaces and allow them to take advantage of intelisense to identify the new methods for objects.

## Motivation ##

The motivation behind the creation of the library was to make development easier and cleaner within the ArcGIS / ArcFM platforms. While the ArcGIS / ArcFM provide there own individual value, there is no connection between the APIs. The library is designed to help bridge that gap. 

## Examples ##

### ESRI ###

----------
**LINQ** is now supported on those interfaces that require recursion.
 
For example, the *IEnumLayer* interface is used to traverse through the layers in the map document. Recursion must be used in order to traverse the group layers and composite layers that can be in the map.

Previously, you'd have to create a recursive method every time you needed to traverse the table of contents. However, now you can use the **Where** method in conjunction with **LINQ** statements.

```c#
/// <summary>
/// Clears the layer definitions.
/// </summary>
/// <param name="map">The map document within ArcMap.</param>
public void ClearDefinitions(IMap map)
{
    foreach (var layerDefinition in map.Where(o => o.Visible).Select(o => (IFeatureLayerDefinition2) o))
    {
		layerDefinition.DefinitionExpression = null;
    }
}
```

There is now the ability to query records from feature classes or tables using the **Fetch** extension method knowing that the implementation handles proper memory management  for the COM objects.

```c#
/// <summary>
///     Updates all of the features 'TIMECREATED' field to the current date time for
///     those features that have NULLs.
/// </summary>
/// <param name="featureClass">The feature class.</param>
/// <returns>
///     Returns a <see cref="int" /> representing the number of records updated.
/// </returns>
public int UpdateTimeCreated(IFeatureClass featureClass)
{
    IQueryFilter filter = new QueryFilterClass();
    filter.WhereClause = "TIMECREATED IS NULL";

    int recordsAffected = featureClass.Fetch(filter, true, feature =>
    {
		feature.Update("TIMECREATED", DateTime.Now);
		feature.SaveChanges();

		return true;
    });

    return recordsAffected;
}
```
Analyzing the changes made within a version has been simplified allowing for filtering for those only those features or tables that are truely needed.
```c#
/// <summary>
/// Validates the updates made within a version by performing 
/// a version difference between the child and it's parent version.
/// </summary>
public void ValidateUpdates(IVersion childVersion, IVersion parentVersion)
{
    // Iterate through all of the differences to feature classes.
     var differences = childVersion.GetDifferences(parentVersion, null, (s, table) => table is IFeatureClass, esriDifferenceType.esriDifferenceTypeUpdateDelete,
                                                                                                                     esriDifferenceType.esriDifferenceTypeUpdateNoChange,
                                                                                                                     esriDifferenceType.esriDifferenceTypeUpdateUpdate);
    
    foreach (var table in differences)
    {
		Console.WriteLine("Table: {0}", table.Key);

		foreach (var differenceRow in table.Value)
		{
			Console.WriteLine("+++++++++++ {0} +++++++++++", differenceRow.OID);

			foreach (var index in differenceRow.FieldIndices.AsEnumerable())
			{
				Console.WriteLine("Original: {0} -> Changed: {1}", differenceRow.Original.GetValue(index, DBNull.Value), differenceRow.Changed.GetValue(index, DBNull.Value));
			}
		}
    }
}
```

### ArcFM ###

----------
**LINQ** is now supported on those interfaces that require recursion.

For example, the *ID8List* interface is used throughout the ArcFM to represent a tree structure of the features and related objects. Recursion must be used in order to traverse the entire structure of the tree. 

Previously, you'd have to create a recursive method every time you needed to traverse the contents of the tree structure. However, now you can use the **Where** method in conjunction with **LINQ** statements.

```c#
/// <summary>
/// Update all of the TAG field with the specified value for all features
/// that reside within the Design Tab.
/// </summary>
/// <param name="tag">The tag information.</param>
public void UpdateTags(string tag)
{
	ID8List list = ArcMap.Application.GetDesignTab();
	foreach (var item in list.Where(o => o is IMMProposedObject).Select(o => (IMMProposedObject) o.Value))
	{
		IMMFieldManager fieldManager = item.FieldManager;
		IMMFieldAdapter fieldAdapter = fieldManager.FieldByName("TAG");
		fieldAdapter.Value = tag;
		item.Update(null);
	}
}
```

Accessing object classes by **Class Model Names** and fields by **Field Model Names** has been simplified.

```c#
/// <summary>
///     Updates the KVA on the transformer unit records.
/// </summary>
/// <param name="transformerClass">The transformer class.</param>
/// <param name="oids">The list of the object ids that identify the features.</param>
/// <param name="kva">The kva rating.</param>
/// <returns>
///     Returns a <see cref="int" /> representing the records affected.
/// </returns>
public int UpdateKva(IFeatureClass transformerClass, int[] oids, int kva)
{
	IRelationshipClass relationshipClas = transformerClass.GetRelationshipClass(esriRelRole.esriRelRoleAny, "TRANSFORMERUNIT");
    int recordsAffected = transformerClass.Fetch(oids, true, feature =>
    {
		// Iterate through all of the related objects for the transformer.
		ISet set = relationshipClas.GetObjectsRelatedToObject((IObject)feature);
		foreach (IRow row in set.AsEnumerable<IRow>())
		{
			row.Update("KVA", kva, true); 	// Use the "Update" extension method because it will only update the field when the values are different.
			row.SaveChanges(); 				// Use the "SaveChanges" extension method because it will only call store when one or more fields have changed.
		}
    });

    return recordsAffected;
}
```

## Installation ##

In the future, installation will be provided by a a nuget package, however, until that time comes you'll need to compile the code.

### Requirements ###
- ArcGIS 9.3.1 SP2
- ArcFM 9.3.1 SP2
- 3.5 SP 1 .NET Framework
- Visual Studio 2013

### Third Party Libraries ###

- log4net 2.0.3

