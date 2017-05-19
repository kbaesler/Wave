# Object Relational Mapping
Simple object-relational mapping abstraction layer for geodatabase feature classes and tables. It converts features and rows into a (lazy) sequence of strongly-typed objects.

## Getting Started
You can begin using the object-relational mappping in 2 easy steps.

1. Create a `class` that inherits from the `Entity` class.
    * Decorate each property on the `class` with the `EntityField` attribute that you want to map to column on a table.
    * Decorate the `class` with the `EntityTable` attribute in order to map the entity to a table.

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
    var idahoSprings = new City
    {
        Name = "Idaho Springs",
        Population = 1740,
        State = "Colorado"
    };

    idahoSprings.Insert(cities);
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
    }
    ```
3. Update: *Flush property changes to the underlying row in the database.*

    Class                   | Method                        | Description
    :-----------------------|:------------------------------|:----------------------|
    `Entity`                | Update()                      | Updates the underlying backing object (either IRow or IFeature)

    ```c#
    var cities = workspace.GetTable(typeof(City));
    var idahoSprings = new City
    {
        Name = "Idaho Springs",
        Population = 1740,
        State = "Colorado"
    };

    idahoSprings.Insert(cities);

    idahoSprings.Population = 250s0;
    idahoSprings.Update();

    ```

 4. Delete: *Remove records from the table using the entity object*.

    Class                   | Method                        | Description
    :-----------------------|:------------------------------|:----------------------|
    `Entity`                | Delete()                      | Deletes the underlying backing object (either IRow or IFeature)

    ```c#
    var cities = workspace.GetTable(typeof(City));
    var idahoSprings = new City
    {
        Name = "Idaho Springs",
        Population = 1740,
        State = "Colorado"
    };

    idahoSprings.Insert(cities);

    idahoSprings.Delete();
    idahoSprings = null;
    ```