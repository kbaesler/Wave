Features
================================
This will serve as a list of all of the features that are currently available in Wave. Some features are important enough to have their own page in the docs, others will simply be listed.

Extending Namespaces
--------------------------
Wave is built using `.NET Extension Methods <http://msdn.microsoft.com/en-us/library/bb383977.aspx>`_ and each object that is extended in the ArcFM or ArcGIS  uses the namespace it exists within the API, which eliminate the need to learn new namespaces and allows Wave's features to be available without adding new namespace delcarations.

Simplifying Complexity
--------------------------
Wave is designed to simplify those tasks that are performed frequently while considering performance, memory consumption and easy of use.

Hierarchical Data Structures
++++++++++++++++++++++++++++++
There are several objects (i.e. ``IEnumLayer``, ``IMap``, ``ID8List``) in ArcFM and ArcGIS APIs that require recursion to obtain all of the data. In the past, you'd have to create a recursive method for iterating the contents of these hierarchical structures. These structures can now be traversed recursively using ``LINQ``.

For example, in ArcGIS traversing the contents of the map document can be simplified using the ``Where`` extension method on the ``IMap`` interface.

.. code-block:: c

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
    
For example, in ArcFM traversing the Design Tab in the ArcFM Attribute Editor can be simplified using the ``Where`` extension method on the ``ID8List`` interface.

.. code-block:: c

    /// <summary>
    /// Update all of the TAG field with the specified value for all features
    /// that reside within the Design Tab.
    /// </summary>
    /// <param name="list">The tree structure for the Design tab on the ArcFM attribute editor.</param>
    /// <param name="tag">The tag information.</param>
    public void UpdateTags(ID8List list, string tag)
    {
        foreach (var item in list.Where(o => o is IMMProposedObject).Select(o => (IMMProposedObject) o.Value))
        {
            IMMFieldManager fieldManager = item.FieldManager;
            IMMFieldAdapter fieldAdapter = fieldManager.FieldByName("TAG");
            fieldAdapter.Value = tag;
            item.Update(null);
        }		
    }

Data Queries
+++++++++++++
One of the major benefits of using the ESRI platform it allows you to perform spatial and attribute based queries against the data to validate and perform analysis. Resulting in this operation being heavily used, which leads to code-duplication and/or memory management issues if used improperly.

The ``ITable`` and ``IFeatureClass`` interfaces have been extended to include ``Fetch`` methods that simplify this operation by abstracting the bulk of the logic and enforcing the proper memory management for the COM objects.

.. code-block:: c	

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
        int recordsAffected = featureClass.Fetch("TIMECREATED IS NULL", true, feature =>          
        {		   
            feature.Update("TIMECREATED", DateTime.Now);
            feature.Store();

            // Return true, to continue to the next feature.
            return true;
        });

        return recordsAffected;
    }

Support Typical Extensions
-------------------------------------
The ArcFM and ArcGIS platform provides multiple extension points and while we cannot address them all we have included abstract implementations for the most common extension made while working with these platforms. 
 
- ``BaseMxCommand``: Used for creating a button within the ArcMap application
- ``BaseGxCommand``: Used for creating a button within the ArcCatalog application.
- ``BaseExtension``: Used for creating an extension within the ArcMap application.
- ``BaseTool``: Used for creating a tool within the ArcMap application.
- ``BaseAbandonAU``: Used for creating a custom trigger for abandoning features in ArcFM.
- ``BaseAttributeAU``: Used for creating a custom trigger for an attribute when the object is created, updated or deleted in ArcFM.
- ``BaseSpecialAU``: Uses for creating a custom trigger for the object when it is created, updated or deleted in ArcFM.
- ``BaseRelationshipAU``: Used for creating a custom trigger for when a relationship is created, updated or deleted in ArcFM.

.. note::

    There are many more that haven't been listed for the sake of brevity.

ArcFM Model Names
------------------------------
The ArcFM Solution provides a way to identify ESRI tables based on a user defined key that they call ArcFM Model Names. These model names can be assigned at the table and field level allow for cross database or generic implementations of customziations. However, they must be accessed using a singleton object that tends to lead to another duplicated class helper in developers code and no connection to the objects that they support.

In order to simplfy the need for duplication, several extension methods were added to those ESRI objects that can be assigned the ArcFM Model Names.

The extension methods for the ``IFeatureClass`` and ``ITable`` interfaces that have been added.

- ``IsAssignedClassModelName``: Used to determine if a class model name(s) has been assigned.
- ``IsAssignedFieldModelName``: Used to determine if a field model name(s) has been assigned.
- ``GetRelationshipClass``: Used to locate the relationship that has been assigned the class model name(s).
- ``GetRelationshipClasses``: Used to gather a list of the relationships that has been assigned the class model name(s).
- ``GetField``: Used to locate the ``IField`` that has been assigned the field model name(s).
- ``GetFields``: Used to gather a list of of the ``IField`` objects that has been assigned the field model name(s).
- ``GetFieldIndex``: Used to locate the field index that has been assigned the field model name(s).
- ``GetFieldIndexes``: Used to gather a list of all of the field indexes that has been assigned the field model name(s).
- ``GetFieldName``: Used to locate the field name that has been assigned the field model name(s).
- ``GetFieldNames``: Used to gather a list of all of the field names that has been assigned the field model name(s).

The extension methods for the ``IWorkspace`` interface that have been added.

- ``IsAssignedDatabaseModelName``: Use to determine if the database model name(s) has been assigned.
- ``GetFeatureClass``: Used to obtain the ``IFeatureClass`` that has been assigned the class model name(s).
- ``GetFeatureClasses``: Used to obtain all of the ``IFeatureClass`` tables that have been assigned the class model name(s).
- ``GetTable``: Used to obtain the ``ITable`` that has been assigned the class model name(s).
- ``GetTables``: Used to obtain all of the ``ITable`` tables that have been assigned the class model name(s).
