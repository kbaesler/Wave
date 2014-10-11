Features
================================
This will serve as a list of all of the features that are currently available in Wave. Some features are important enough to have their own page in the docs, others will simply be listed.

Recursion with LINQ
----------------------
There are several objects that require recursion to obtain all of the information in both ArcFM and ArcGIS. These objects can now be traversed recursively using ``LINQ``.
 
The ``IEnumLayer`` interface is used to traverse through the layers in the ArcGIS map document. Recursion must be used in order to traverse the group layers and composite layers that can be in the map. Previously, you'd have to create a recursive method every time you needed to traverse the table of contents. However, now you can use the ``Where`` method in conjunction with ``LINQ`` statements.

.. code-block:: c
	:linenos:	

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

The ``ID8List`` interface is used throughout the ArcFM to represent a tree structure of the features and related objects. Recursion must be used in order to traverse the entire structure of the tree. Previously, you'd have to create a recursive method every time you needed to traverse the contents of the tree structure. However, now you can use the ``Where`` method in conjunction with ``LINQ`` statements.

.. code-block:: c
	:linenos:	

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

Memory Management
------------------------------
The management of memory is always a constant concern while working within the ArcGIS and ArcFM environment, thus several extension methods have been created to eliminate these concerns by wrapping up the proper memory management while still providing the information necessary to the developer.

.. code-block:: c
	:linenos:	

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
		   feature.Store();
	         
		   // Return true, to continue to the next feature.
		   return true;
	    });
	
	    return recordsAffected;
	}

Common Base Classes 
-----------------------------------
The ArcFM and ArcGIS platform provides many extension points and while we cannot address them all we have included base class implementations for the most common extension made while working with these platforms. 
 
* ``BaseMxCommand``: Used for creating a button within the ``ArcMap`` application
* ``BaseGxCommand``: Used for creating a button within the ``ArcCatalog`` application.
* ``BaseExtension``: Used for creating an extension within the ``ArcMap`` application.
* ``BaseTool``: Used for creating a tool within the ``ArcMap`` application.
* ``BaseAbandonAU``: Used for creating a custom trigger for abandoning features in ArcFM.
* ``BaseAttributeAU``: Used for creating a custom trigger for an attribute when the object is created, updated or deleted in ArcFM.
* ``BaseSpecialAU``: Uses for creating a custom trigger for the object when it is created, updated or deleted in ArcFM.
* ``BaseRelationshipAU``: Used for creating a custom trigger for when a relationship is created, updated or deleted in ArcFM.

There are many more that haven't been listed for the sake of brevity.

Class & Field Model Names
------------------------------
Accessing object classes by ``Class Model Names`` and fields by ``Field Model Names`` has been simplified due to the extension methods added to the objects and interfaces.

.. code-block:: c
	:linenos:	

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

Best Practices
---------------

Process Framework Extensions
-------------------------------