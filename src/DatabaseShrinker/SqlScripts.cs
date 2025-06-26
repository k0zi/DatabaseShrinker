namespace DatabaseShrinker;

public class SqlScripts
{
    private readonly string checkDbState;
    private readonly string getDatabaseFiles;
    private readonly string listUserDatabases;
    private readonly string shrinkDatabaseFile;
    private readonly string queryDbSizes;
    private readonly string simpleLog;


    public SqlScripts()
    {
        checkDbState = DatabaseShrinker.ResourceLoader.GetResource("DatabaseShrinker.SqlScripts.CheckDbccState.sql");
        getDatabaseFiles = DatabaseShrinker.ResourceLoader.GetResource("DatabaseShrinker.SqlScripts.GetDatabaseFiles.sql");
        listUserDatabases = DatabaseShrinker.ResourceLoader.GetResource("DatabaseShrinker.SqlScripts.ListUserDatabases.sql");
        shrinkDatabaseFile = DatabaseShrinker.ResourceLoader.GetResource("DatabaseShrinker.SqlScripts.ShinkDatabaseFile.sql");
        queryDbSizes = DatabaseShrinker.ResourceLoader.GetResource("DatabaseShrinker.SqlScripts.QueryDbSizes.sql");
        simpleLog = DatabaseShrinker.ResourceLoader.GetResource("DatabaseShrinker.SqlScripts.SimpleLog.sql");
    }
    
    public string CheckDbccState => checkDbState;
    public string GetDatabaseFiles => getDatabaseFiles;
    public string ListUserDatabases => listUserDatabases;
    public string ShrinkDatabaseFile => shrinkDatabaseFile;
    public string QueryDbSizes => queryDbSizes;
    public string SimpleLog => simpleLog;
}