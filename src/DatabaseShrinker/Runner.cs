using System.Diagnostics;
using DatabaseShrinker;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Spectre.Console;

public class Runner(ILogger logger,
    Func<string, ISqlConnector> sqlConnectorFactory) : IRunner
{
    public void RunConnectionString(string connectionString, ShrinkSetting setting)
    {
        ISqlConnector connector = sqlConnectorFactory(connectionString);;
        bool isValid = true;
        AnsiConsole.Markup($"Validating:[gold1]{connectionString}[/]     ");
        isValid = connector.IsConnectionValid();
        if (isValid)
        {
            AnsiConsole.MarkupLine($"[green]Ok[/]");
            Log($"Connection string: {connectionString} is valid", setting.Log);
        }
        else
        {
            AnsiConsole.MarkupLine($"[red]Failed[/]");
            Log($"Connection string: {connectionString} is not valid", setting.Log);
        }

        if (!isValid)
        {
            return;
        }

        

        string[] databases = [];
        AnsiConsole.Markup("Getting user database(s)");
        databases = connector.ListUserDatabases();
        if (databases.Length == 0)
        {
            AnsiConsole.MarkupLine("   [red]No database![/]");
            Log($"Database not found on server", setting.Log);
        }

        if (databases.Length == 0)
        {
            return;
        }
        var dbSizes = DisplayDatabaseSizes(connector)
            .Where(ds => ds.FreeSizeMb > 1024)
            .ToArray();
        bool confirmation = setting.AutoConfirm ? setting.AutoConfirm : AnsiConsole.Prompt(
            new TextPrompt<bool>("Shrink database(s)?")
                .AddChoice(true)
                .AddChoice(false)
                .DefaultValue(true)
                .WithConverter(choice => choice ? "y" : "n"));
        if (!confirmation)
        {
            return;
        }
        
        bool largeOnly = setting.ShrinkOnlyLarge ? setting.ShrinkOnlyLarge :  AnsiConsole.Prompt(
            new TextPrompt<bool>("Only large database(s)?")
            .AddChoice(true)
            .AddChoice(false)
            .DefaultValue(true)
            .WithConverter(choice => choice ? "y" : "n"));
        
        var databaseFiles = GetDatabaseFiles(connector, databases);
        if (largeOnly)
        {
            databaseFiles = databaseFiles
                .Where(dp => dbSizes.Any(ds => ds.DbName == dp.Database && ds.FileName == dp.File))
                .ToArray();
        }
        AnsiConsole.Markup($"[darkcyan]Shrinking database files[/]");
        Log($"Shrinking database files", setting.Log);
        AnsiConsole.Progress()
            .Start(ctx =>
            {
                var dbccProcesses = new List<DbccProcess>();
                var sqlTasks = new List<Task>();
                foreach (var databasePair in databaseFiles
                             .Where(f => f.DbFileType == DbFileType.Data))
                {
                    var shrinkProcess = connector.ShrinkDatabaseFile(databasePair.Database, databasePair.File);
                    var task = ctx.AddTask($"[darkcyan]{databasePair.Database} -> {databasePair.File}[/]", autoStart:false);
                    dbccProcesses.Add(new DbccProcess(shrinkProcess.Item1, task));
                    sqlTasks.Add(shrinkProcess.Item2);
                    Log($"Database: {databasePair.Database} -> {databasePair.File}", setting.Log);
                }

                foreach (var sqlTask in sqlTasks)
                {
                    sqlTask.Start();
                }

                foreach (var dbccProcess in dbccProcesses)
                {
                    dbccProcess.ProgressTask.StartTask();
                }

                while(!ctx.IsFinished || sqlTasks.Any(t=>!t.IsCompleted)) 
                {
                    Thread.Sleep(50);
                    foreach (var dbccProcess in dbccProcesses)
                    {
                        var prevState = dbccProcess.Progress;
                        dbccProcess.Progress = connector.CheckDbccState(dbccProcess.SessionId);
                        Debug.WriteLine($"Prev: {prevState}; State {dbccProcess.Progress}");
                        var increment = dbccProcess.Progress - prevState;
                        dbccProcess.ProgressTask.Increment(increment);
                    }
                }

                Task.WaitAll(sqlTasks.ToArray());
            });
        bool dropLog = setting.DropLog ? setting.DropLog :
            AnsiConsole.Prompt(
                new TextPrompt<bool>("Set recovery model to simple and drop transaction log?")
                    .AddChoice(true)
                    .AddChoice(false)
                    .DefaultValue(true)
                    .WithConverter(choice => choice ? "y" : "n"));
        if (dropLog)
        {
            var logFiles = databaseFiles.Where(f => f.DbFileType == DbFileType.Log);
            AnsiConsole.Markup($"[darkcyan]Dropping transaction log[/]");
            Log($"Dropping transaction log", setting.Log);
            foreach (var logFile in logFiles)
            {
                try
                {
                    connector.DropLog(logFile);
                    Log($"Dropping transaction log: {logFile.Database} -> {logFile.File}", setting.Log);
                }
                catch (SqlException)
                {
                    Log($"Failed to drop transaction log: {logFile.Database} -> {logFile.File}", setting.Log);
                }
            }
        }
        DisplayDatabaseSizes(connector);
    }

    private static DbSize[] DisplayDatabaseSizes(ISqlConnector connector)
    {
        var result = connector.QueryDbSizes();
        var table = new Table().LeftAligned().Border(TableBorder.Rounded);
        table.AddColumn("DbName");
        table.AddColumn("FileName");
        table.AddColumn("Type");
        table.AddColumn("Current Size MB");
        table.AddColumn("Free Space MB");
        AnsiConsole.Live(table)
            .Start(ctx =>
            {
                foreach (var dbSize in result)
                {

                    var diffText = new Markup(dbSize.FreeSizeMb > 10240 ? $"[red]{dbSize.FreeSizeMb.ToString()}[/]" : dbSize.FreeSizeMb > 1024 ? $"[gold1]{dbSize.FreeSizeMb.ToString()}[/]" : $"[green]{dbSize.FreeSizeMb.ToString()}[/]");
                    table.AddRow(new Markup(dbSize.DbName), 
                        new Markup(dbSize.FileName), 
                        new Markup(dbSize.TypeDesc), 
                        new Markup(dbSize.CurrentSizeMb.ToString()),
                        diffText);
                    ctx.Refresh();
                }
            });
        return result.ToArray();
    }

    public void RunConnectionStrings(string jsonPath, ShrinkSetting setting)
    {
        var strings = new ConnectionStringLoader(jsonPath).GetAllConnectionStrings();
        RunConnectionStrings(strings, setting);
    }
    private void RunConnectionStrings(string[] connectionStrings, ShrinkSetting setting)
    {
        foreach (var connectionString in connectionStrings)
        {
            RunConnectionString(connectionString, setting);
        }
    }

    private DatabaseFile[] GetDatabaseFiles(ISqlConnector connector, string[] databases)
    {
        var pairs = new List<DatabaseFile>();
        foreach (var database in databases)
        {
            return connector.GetDatabaseFiles(database);
        }

        return pairs.ToArray();
    }
    
    private void Log(string message, bool isEnabled)
    {
        if(isEnabled)
            logger.LogInformation(message);
    }
}