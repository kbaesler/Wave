ie-v.exe
========
A command line utility that will export or import the version differences of the version given database that is specified by the connection file.

Export
------
The command line utility will create a directory structure that is the same as the database and store the exported XML files in the directory location that reflects the location of the object in the database.

- `/f:` - The path to the `.sde` connection file.
- `/d:` - The path to the output directory that will contain the delta file.
- `/v:` - The name of the version.
- `/t:Export` - A constant used to indicate that the delta file should be exported to the directory (specified by `/d`).
