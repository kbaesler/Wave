# Object Relational Mapping
Simple object-relational mapping abstraction layer for geodatabase feature classes and tables. It converts features and rows into a (lazy) sequence of strongly-typed objects.

## Getting Started
You can begin using the object-relational mappping in 2 easy steps.

1. Create a `class` that inherits from the `Entity` class.
    * Decorate each property in the `class` with the `EntityField` attribute that you want to have mapped to column on a table.
    * Decorate the `class` with the `EntityTable` attribute that you want to have mapped to a table.

    ```c#
    [EntityTable("CITY")]
    public class City : Entity
    {
        [EntityField("NAME")]
        public string Name {get;set;}

        [EntityField("POPULATION")]
        public int? Population {get;set;}

        [EntityField("STATE")]
        public string State {get;set;}

        [EntityField("DESCRIPTION")]
        public string Description {get;set}
    }
    ```
2. Use the **CRUD** extension methods that have been exposed on the `ITable` and `IFeatureClass` interfaces and `Entity` class.

## CRUD
An entity object can be used to **C**-reate, **R**-ead, **U**-pdate and **D**-elete either `IRow` or `IFeature` objects by using methods exposed on the `Entity` object or (`ITable` and `IFeatureClass`) interfaces.
  
1. Create: *Create new records using entity objects*.

    Class                   | Method                        | Description
    :-----------------------|:------------------------------|:----------------------|
    `Entity`                | Insert(ITable)                | Inserts the entity object into the table using an insert cursor.
    `Entity`                | Insert(IFeatureClass)         | Inserts the entity object into the feature class using an insert cursor. 

    ```c#
    var cities = workspace.GetTable(typeof(City));
    var denver = new City
    {
        Name = "Denver",
        Population = 663862,
        State = "Colorado",
        Description = "The capital of Colorado."
    };

    denver.Insert(cities);
    ```
2. Read: *Load data from a table into entity objects.*

    Interface               | Method                        | Description
    :-----------------------|:------------------------------|:----------------------|
    `ITable`                | Map(IQueryFilter)             | Reads the database rows as a (lazily-evaluated) sequence of objects that are converted into the entity type.
    `IFeatureCLass`         | Map(IQueryFilter)             | Reads the database features as a (lazily-evaluated) sequence of objects that are converted into the entity type.

    ```c#
    var filter = new QueryFilter();
    filter.Where = "State = 'Colorado';

    var cities = workspace.GetTable(typeof(City));
    foreach (var city in cities.Map<City>(filter)
    {
        Console.WriteLine("Name: {0}" , city.Name);
        Console.WriteLine("POP: {0}" , city.Population);
        Console.WriteLine("State: {0}" , city.State);
        Console.WriteLine("Description : {0}", city.Description);
    }
    ```
3. Update: *Flush property changes to the underlying row in the database.*

    Class                   | Method                        | Description
    :-----------------------|:------------------------------|:----------------------|
    `Entity`                | Update()                      | Updates the underlying backing object (either IRow or IFeature)

    ```c#
    var cities = workspace.GetTable(typeof(City));
    var filter = new QueryFilter();
    filter.Where = "Name = 'Denver'";

    var denver = cities.Map<City>(filter).SingleOrDefault();
    denver.Description = "Denver, the capital of Colorado, is an American metropolis dating to the Old West era. Larimer Square, the city’s oldest block, features landmark 19th-century buildings.";
    denver.Update();

    ```

 4. Delete: *Remove records from the table using the entity object*.

    Class                   | Method                        | Description
    :-----------------------|:------------------------------|:----------------------|
    `Entity`                | Delete()                      | Deletes the underlying backing object (either IRow or IFeature)

    ```c#
    var cities = workspace.GetTable(typeof(City));
    var filter = new QueryFilter();
    filter.Where = "Name = 'Denver'";

    var denver = cities.Map<City>(filter).SingleOrDefault();
    denver.Delete();
    denver = null;
    ```