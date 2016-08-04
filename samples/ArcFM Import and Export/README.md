# ie-xml.exe
A command line utility that will export or import the ArcFM Properties XML for all of the feature classes, tables and relationships in the given database that is specified by the connection file.

> **Tips**
  1. You should always run the utility on a machine that has the ArcFM Solution Desktop, ArcGIS for Desktop, and any customizations that have been configured on the database connections. Otherwise, the names of the components will all come out as `UNREGISTERED PROGRAM`.
  2. The user and password should be saved in the `sde` connection files.
  3. The user that is saved in the `sde` connection files should be either the `sde` or `data owner` user.

## Export
The command line utility will create a directory structure that is the same as the database and store the exported `XML` files in the directory location that reflects the location of the object in the database.

In addition to exporting the individual `XML` files, an `XML` and `CSV` file that contains all of the properties will be created in the root of the directory specified (`/d`) with the name of the `sde` connection file.

- `/f:` - The path to the `.sde` connection file.
- `/d:` - The path to the output directory that will contain the XML files.
- `/t:Export` - A constant used to indicate that the XML files should be exported to the directory (specified by `/d`).

``` bat
ie-xml.exe /f:"C:\IEXML\Demo\Database1.sde" /d:"C:\IEXML\Demo" /t:Export

```

## Import
The command line utility will create a directory structure that is the same as the database and store the exported XML files in the directory location that reflects the location of the object in the database.

- `/f:` - The path to the `.sde` connection file.
- `/d:` - The path to the directory (and sub-directories) that will contain the XML files.
- `/t:Import` - A constant used to indicate that the XML files in the directory (specified by `/d`) should be imported.

``` bat
ie-xml.exe /f:"C:\IEXML\Demo\Database1.sde" /d:"C:\IEXML\Demo\Database1" /t:Import

```

## Compare
The command line utility will compare the specified `CSV` files output a resultant `CSV` of the differences.

- `/f:` - The path to the `.csv` files that will be compared. (The `.csv` files must have the same schema (i.e. generated from the `/t:Export`)).
- `/d:` - The path to the directory that will contain the comparison results.
- `/t:Compare` - A constant used to indicate that the CSV files (specified by `/f`) that will be compared.

``` bat
ie-xml.exe /f:"C:\IEXML\Demo\Database1.csv" /f:"C:\IEXML\Demo\Database2.csv" /d:"C:\IEXML\Demo" /t:Compare

```

> There will be 2 files generated as results of the comparison because the comparison is bi-directional. Meaning, it will compare `Database1.csv` against `Database2.csv` for differences, and `Database2.csv` against `Database1.csv` for differences. The resultant files will be named based on which file that contains the set differences of two files. For example, in the above example the 2 files that will be produced will be named
 - `Database1 vs Database2.csv`
 - `Database2 vs Database1.csv`
