namespace DatabaseShrinker;

public interface ISqlConnectorFactory
{
    ISqlConnector Create(string connectionString);
}