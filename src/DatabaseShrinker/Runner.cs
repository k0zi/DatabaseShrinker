using DatabaseShrinker;
using Spectre.Console;

public class Runner
{
    public void RunConnectionString(string connectionString)
    {
        SqlConnector connector = null;
        bool isValid = true;
        AnsiConsole.Status()
            .Start("Configuring..", ctx =>
            {
                connector = new SqlConnector(connectionString, new SqlScripts());
                AnsiConsole.WriteLine($"Validating: {connectionString}");
                ctx.Status("Validating connection string");
                ctx.Spinner(Spinner.Known.Dots);
                ctx.SpinnerStyle(Style.Parse("green"));
                isValid = connector.IsConnectionValid();
                Thread.Sleep(1000);
                if (isValid)
                {
                    AnsiConsole.MarkupLine($"[green]Ok[/]");
                }
                else
                {
                    AnsiConsole.MarkupLine($"[red]Failed[/]");
                }
            });
        if (!isValid)
        {
            return;
        }

        string[] databases = [];
        AnsiConsole.Status()
            .Start("Getting database(s)..", ctx =>
            {
                AnsiConsole.MarkupLine("Getting user database(s)");
                ctx.Status("Quering...");
                ctx.Spinner(Spinner.Known.Dots);
                ctx.SpinnerStyle(Style.Parse("green"));
                databases = connector.ListUserDatabases();
                if (databases.Length == 0)
                {
                    AnsiConsole.MarkupLine("[red]No database![/]");
                }
            });
        if (databases.Length == 0)
        {
            return;
        }

        foreach (var database in databases)
        {
            var files = connector.GetDatabaseFiles(database);
            foreach (var file in files)
            {
                AnsiConsole.Progress()
                    .Start(proc =>
                    {
                        var task = proc.AddTask($"[darkcyan]Shrinking {file} of {database}[/]");
                        Task.Factory.StartNew(() => connector.ShrinkDatabaseFile(database, file));
                        double state = 0.0;
                        while (state < 100.0 && !proc.IsFinished)
                        {
                            var prevState = state;
                            state = connector.CheckDbccState();
                            var increment = state - prevState;
                            task.Increment((double)increment);
                            Task.Delay(200);
                        }
                    });
            }
        }
    }

    public void RunConnectionStrings(string[] connectionStrings)
    {
        foreach (var connectionString in connectionStrings)
        {
            RunConnectionString(connectionString);
        }
    }
}