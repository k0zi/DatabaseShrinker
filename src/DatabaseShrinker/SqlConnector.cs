using System.Data;
using Microsoft.Data.SqlClient;

namespace DatabaseShrinker;

public class SqlConnector(string connectionString,
    SqlScripts sqlScripts)
{
    private SqlConnection? _connection = null;
    public bool IsConnectionValid()
    {
        _connection = new SqlConnection(connectionString);
        try
        {
            _connection.Open();
        }
        catch (Exception)
        {
            return false;
        }
        return true;
    }

    private void GuardConnection()
    {
        if (_connection == null)
        {
            if (!IsConnectionValid())
                throw new InvalidOperationException("Connection is not valid!");
        }
    }
    public string[] GetDatabaseFiles(string databaseName)
    {
        GuardConnection();
        if(_connection?.State != ConnectionState.Open)
            _connection?.Open();
        var command = new SqlCommand(string.Format(sqlScripts.GetDatabaseFiles, databaseName), _connection);
        using var reader = command.ExecuteReader();
        var files = new List<string>();
        while (reader.Read())
        {
            files.Add(reader.GetString(0));
        }
        return files.ToArray();
    }

    public string[] ListUserDatabases()
    {
        GuardConnection();
        if(_connection?.State != ConnectionState.Open)
            _connection?.Open();
        var command = new SqlCommand(sqlScripts.ListUserDatabases, _connection);
        using var reader = command.ExecuteReader();
        var databases = new List<string>();
        while (reader.Read())
        {
            databases.Add(reader.GetString(0));
        }
        return databases.ToArray();
    }

    public Tuple<Guid,Task> ShrinkDatabaseFile(string database, string databaseFile)
    {
        var connection = new SqlConnection(connectionString);
        connection.Open();
        var sqlTask = Task.Factory.StartNew(() =>
        {
            var command = new SqlCommand(string.Format(sqlScripts.ShrinkDatabaseFile, database, databaseFile),
                connection);
            command.CommandTimeout = 3600;
            command.ExecuteNonQuery();
        });
        return new Tuple<Guid, Task>(connection.ClientConnectionId, sqlTask);
    }

    public double CheckDbccState(Guid clientId)
    {
        GuardConnection();
        if(_connection?.State != ConnectionState.Open)
            _connection?.Open();
        var command = new SqlCommand(string.Format(sqlScripts.CheckDbccState, clientId), _connection);
        using var reader = command.ExecuteReader();
        return reader.Read() ? reader.GetFloat(0) : 100.00001;
    }

    public List<DbSize> QueryDbSizes()
    {
        GuardConnection();
        if(_connection?.State != ConnectionState.Open)
            _connection?.Open();
        var query = new SqlCommand(sqlScripts.QueryDbSizes, _connection);
        using var reader = query.ExecuteReader();
        var result = new List<DbSize>();
        while (reader.Read())
        {
            result.Add(new DbSize(DbName: reader.GetString("DbName"),FileName: reader.GetString("FileName"),TypeDesc: reader.GetString("type_desc"),CurrentSizeMb:reader.GetDecimal("CurrentSizeMB"), FreeSizeMb:reader.GetDecimal("FreeSpaceMB")));
        }

        return result;
    }
}