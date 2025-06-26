namespace DatabaseShrinker;

public interface ISqlConnector
{
    bool IsConnectionValid();
    DatabaseFile[] GetDatabaseFiles(string databaseName);
    string[] ListUserDatabases();
    Tuple<int,Task> ShrinkDatabaseFile(string database, string databaseFile);
    double CheckDbccState(int sessionId);
    List<DbSize> QueryDbSizes();
    void DropLog(DatabaseFile databaseFile);
}