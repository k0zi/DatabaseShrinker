namespace DatabaseShrinker;

public class SqlScripts
{
    private readonly string checkDbState;
    private readonly string getDatabaseFiles;
    private readonly string listUserDatabases;
    private readonly string shrinkDatabaseFile;


    public SqlScripts()
    {
        checkDbState = DatabaseShrinker.ResourceLoader.GetResource("DatabaseShrinker.SqlScripts.CheckDbccState.sql");
        getDatabaseFiles = DatabaseShrinker.ResourceLoader.GetResource("DatabaseShrinker.SqlScripts.GetDatabaseFiles.sql");
        listUserDatabases = DatabaseShrinker.ResourceLoader.GetResource("DatabaseShrinker.SqlScripts.ListUserDatabases.sql");
        shrinkDatabaseFile = DatabaseShrinker.ResourceLoader.GetResource("DatabaseShrinker.SqlScripts.ShinkDatabaseFile.sql");
    }
    
    public string CheckDbccState => checkDbState;
    public string GetDatabaseFiles => getDatabaseFiles;
    public string ListUserDatabases => listUserDatabases;
    public string ShrinkDatabaseFile => shrinkDatabaseFile;
}