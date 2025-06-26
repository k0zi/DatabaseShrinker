# DatabaseShrinker
Shrinks all databases on a database server

## How to use
Use one connection string
```
-c "connection string"
```
Use a connections.json file to use multiple connection string
```
-s "path/of/a/connections.json" : adds a json file with connection string(s) to shrink
```
And the rest
```
-v : show version
-h : shows this help
```
Additional settings
```
-l : log to file
-y : skip confirmation
-a : shrink all database(s)
-o : shrink only large database(s)
-d : set simple recovery mode and drop transaction log
```