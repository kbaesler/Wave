# Extensions
The ArcFM and ArcGIS platform provides multiple extension points and while we cannot address them all we have included abstract implementations for the most common extensions made while working with these software packages.

Class                  | Description
:----------------------|:-------------
 `BaseMxCommand`     | Creating a button within the ArcMap application.                                   
 `BaseGxCommand`     | Creating a button within the ArcCatalog application.                               
 `BaseExtension`     | Creating an extension within the ArcMap application.                                
 `BaseTool`          | Creating a tool within the ArcMap application.                                     
 `BaseAbandonAU`     | Creating a custom trigger for abandoning features.                            
 `BaseAttributeAU`   | Creating a custom trigger for a field when the object is created, updated or deleted.
 `BaseSpecialAU`     | Creating a custom trigger for the object when it is created, updated or deleted.    
 `BaseRelationshipAU`| Creating a custom trigger for when a relationship is created, updated or deleted.   
 `BasePxSubtask`     | Creating a sub-routine that can be assigned to tasks within the Process Framework.  
 `...`                 | *There are many more that haven't been listed for the sake of brevity.*

## Model Names
The ArcFM Solution provides a way to identify ESRI tables and fields based on a user defined key that are call Class, Field and Database Model Names. These model names can be for cross-database or generic implementations. However, they must be accessed using a singleton object, which tends to lead to the creation of class helper.

In order to simplify the accessing of model name information, several extension methods were added to the ESRI objects that support ArcFM Model Names.

The extension methods for the `IFeatureClass` and `ITable` interfaces that have been added.

Method                        | Description                                                                                       
:-----------------------------|:---------------------------------------------------------------------------------------------------
`IsAssignedClassModelName`  | Used to determine if a class model name(s) has been assigned.                                     
`IsAssignedFieldModelName`  | Used to determine if a field model name(s) has been assigned.                                     
`GetRelationshipClass`      | Used to locate the relationship that has been assigned the class model name(s).                   
`GetRelationshipClasses`    | Used to gather a list of the relationships that has been assigned the class model name(s).        
`GetField`                  | Used to locate the `IField` that has been assigned the field model name(s).                     
`GetFields`                 | Used to gather a list of of the `IField` objects that has been assigned the field model name(s).
`GetFieldIndex`             | Used to locate the field index that has been assigned the field model name(s).                    
`GetFieldIndexes`           | Used to gather a list of all of the field indexes that has been assigned the field model name(s).
`GetFieldName`              | Used to locate the field name that has been assigned the field model name(s).                     
`GetFieldNames`             | Used to gather a list of all of the field names that has been assigned the field model name(s).   
 `...`                         | *There are many more that haven't been listed for the sake of brevity.*

A list of extension methods for the `IWorkspace` interface that have been added.

 Method                           | Description                                                                                        
:---------------------------------|:---------------------------------------------------------------------------------------------------
`IsAssignedDatabaseModelName`   | Use to determine if the database model name(s) has been assigned.                                  
`GetFeatureClass`               | Used to obtain the `IFeatureClass` that has been assigned the class model name(s).               
`GetFeatureClasses`             | Used to obtain all of the `IFeatureClass` tables that have been assigned the class model name(s).
`GetTable`                      | Used to obtain the `ITable` that has been assigned the class model name(s).                      
`GetTables`                     | Used to obtain all of the `ITable` tables that have been assigned the class model name(s).   
 `...`                         | *There are many more that haven't been listed for the sake of brevity.*    
