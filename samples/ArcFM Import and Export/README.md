ie-xml.exe
========
A command line utility that will export or import the ArcFM Properties XML for all of the feature classes, tables and relationships in the given database that is specified by the connection file. 

Export
------
The command line utility will create a directory structure that is the same as the database and store the exported XML files in the directory location that reflects the location of the object in the database.

- `/f:` - The path to the `.sde` connection file.
- `/d:` - The path to the output directory that will contain the XML files.
- `/t:Export` - A constant used to indicate that the XML files should be exported to the directory (specified by `/d`).

Import
------
The command line utility will create a directory structure that is the same as the database and store the exported XML files in the directory location that reflects the location of the object in the database.

- `/f:` - The path to the `.sde` connection file.
- `/d:` - The path to the directory (and sub-directories) that will contain the XML files.
- `/t:Import` - A constant used to indicate that the XML files in the directory (specified by `/d`) should be imported.
