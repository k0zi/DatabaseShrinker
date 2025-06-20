using System.Diagnostics;
using DatabaseShrinker;
using Spectre.Console;

public class Runner
{
    public void RunConnectionString(string connectionString)
    {
        SqlConnector connector = null;
        bool isValid = true;
        connector = new SqlConnector(connectionString, new SqlScripts());
        AnsiConsole.Markup($"Validating:[gold1]{connectionString}[/]     ");
        isValid = connector.IsConnectionValid();
        if (isValid)
        {
            AnsiConsole.MarkupLine($"[green]Ok[/]");
        }
        else
        {
            AnsiConsole.MarkupLine($"[red]Failed[/]");
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
        }

        if (databases.Length == 0)
        {
            return;
        }
        var dbSizes = DisplayDatabaseSizes(connector)
            .Where(ds => ds.FreeSizeMb > 1024)
            .ToArray();
        var confirmation = AnsiConsole.Prompt(
            new TextPrompt<bool>("Shrink database(s)?")
                .AddChoice(true)
                .AddChoice(false)
                .DefaultValue(true)
                .WithConverter(choice => choice ? "y" : "n"));
        if (!confirmation)
        {
            return;
        }

        var largeOnly = AnsiConsole.Prompt(
            new TextPrompt<bool>("Only large database(s)?")
            .AddChoice(true)
            .AddChoice(false)
            .DefaultValue(true)
            .WithConverter(choice => choice ? "y" : "n"));
        
        var databasePairs = GetDatabasePairs(connector, databases);
        if (largeOnly)
        {
            databasePairs = databasePairs
                .Where(dp => dbSizes.Any(ds => ds.DbName == dp.Database && ds.FileName == dp.File))
                .ToArray();
        }
        AnsiConsole.Markup($"[darkcyan]Shrinking database files[/]");
        AnsiConsole.Progress()
            .Start(ctx =>
            {
                var dbccProcesses = new List<DbccProcess>();
                var sqlTasks = new List<Task>();
                foreach (var databasePair in databasePairs)
                {
                    var shrinkProcess = connector.ShrinkDatabaseFile(databasePair.Database, databasePair.File);
                    var task = ctx.AddTask($"[darkcyan]{databasePair.Database} -> {databasePair.File}[/]", autoStart:false);
                    dbccProcesses.Add(new DbccProcess(shrinkProcess.Item1, task));
                    sqlTasks.Add(shrinkProcess.Item2);
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

        DisplayDatabaseSizes(connector);
    }

    private static DbSize[] DisplayDatabaseSizes(SqlConnector connector)
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

    public void RunConnectionStrings(string[] connectionStrings)
    {
        foreach (var connectionString in connectionStrings)
        {
            RunConnectionString(connectionString);
        }
    }

    private DatabasePair[] GetDatabasePairs(SqlConnector connector, string[] databases)
    {
        var pairs = new List<DatabasePair>();
        foreach (var database in databases)
        {
            var files = connector.GetDatabaseFiles(database);
            foreach (var file in files)
            {
                pairs.Add(new DatabasePair(database, file));
            }
        }

        return pairs.ToArray();
    }
}