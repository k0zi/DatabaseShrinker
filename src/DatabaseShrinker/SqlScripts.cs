namespace DatabaseShrinker;

public class SqlScripts
{
    private readonly string _checkDbState = DatabaseShrinker.ResourceLoader.GetResource("DatabaseShrinker.SqlScripts.CheckDbccState.sql");
    private readonly string _getDatabaseFiles = DatabaseShrinker.ResourceLoader.GetResource("DatabaseShrinker.SqlScripts.GetDatabaseFiles.sql");
    private readonly string _listUserDatabases = DatabaseShrinker.ResourceLoader.GetResource("DatabaseShrinker.SqlScripts.ListUserDatabases.sql");
    private readonly string _shrinkDatabaseFile = DatabaseShrinker.ResourceLoader.GetResource("DatabaseShrinker.SqlScripts.ShinkDatabaseFile.sql");
    private readonly string _queryDbSizes = DatabaseShrinker.ResourceLoader.GetResource("DatabaseShrinker.SqlScripts.QueryDbSizes.sql");
    private readonly string _simpleLog = DatabaseShrinker.ResourceLoader.GetResource("DatabaseShrinker.SqlScripts.SimpleLog.sql");


    public string CheckDbccState => _checkDbState;
    public string GetDatabaseFiles => _getDatabaseFiles;
    public string ListUserDatabases => _listUserDatabases;
    public string ShrinkDatabaseFile => _shrinkDatabaseFile;
    public string QueryDbSizes => _queryDbSizes;
    public string SimpleLog => _simpleLog;
}