To use this project you must setup a few thing yourself:

1)
Setup a MSSQL server.

2)
To define the MSSQL server, add a file named 'connections.config' in 'CrawlerLibrary' and the same in 'Crawler' (or set a build event to copy it)

Format the content as this:
```
<connectionStrings>
  <add name="CrawlerConnectionString" connectionString="{your connections string here}" />
</connectionStrings>
```

EG like this:
```
<connectionStrings>
  <add name="CrawlerConnectionString" connectionString="Server=tcp:indexer.database.windows.net,1433;Initial Catalog=IndexerDB;Persist Security Info=False;User ID={user};Password={password};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=60;" providerName="System.Data.SqlClient" />
</connectionStrings>
```

3)
Setup a Solr server, schemaless

4)
Use the address of the server in 'Crawler' -> 'Program.cs' 
line: Startup.Init<HTMLContent>("http://XXX.XXX.XXX.XXX:PORT/Dir/To/Core");

5)
If you want to edit the starting site it is in the Context -> DBInitializer

QQQQ is inserted the places you might want to edit