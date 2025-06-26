namespace DatabaseShrinker;

public class SqlConnectorFactory(SqlScripts sqlScripts) : ISqlConnectorFactory
{
    public ISqlConnector Create(string connectionString)
    {
        return new SqlConnector(connectionString, sqlScripts);
    }
}