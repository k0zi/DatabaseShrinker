using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using Spectre.Console;

namespace DatabaseShrinker;

public static class Help
{
    public static string GetHelp() => @"Database Shrinker
Manual 
-c ""connection string"" : a single connection string to shrink
-s ""path/of/a/connections.json"" : a json file with connection string(s) to shrink
-v : show version
-h : shows this help

Options
-l : log to file
-y : skip confirmation
-a : shrink all database(s)
-o : shrink only large database(s)
-d : set simple recovery mode and drop transaction log";

    public static string GetVersion()
    {
        var assembly = System.Reflection.Assembly.GetExecutingAssembly();
        return assembly.GetName().Version!.ToString();
    }
    

    public static Command[] GetCommands(ILogger logger, Func<string, ISqlConnector> sqlConnectorFactory) => [
        new("-c", (string input, ShrinkSetting setting) => new Runner(logger, sqlConnectorFactory).RunConnectionString(input, setting)),
        new("-s", (string input, ShrinkSetting setting) => new Runner(logger, sqlConnectorFactory).RunConnectionStrings(input, setting)),
        new("-v", (string input, ShrinkSetting setting) => AnsiConsole.WriteLine("Version: {0}", DatabaseShrinker.Help.GetVersion())),
        new("-h", (string input, ShrinkSetting setting) => AnsiConsole.WriteLine(DatabaseShrinker.Help.GetHelp())),
    ];

    public static ShrinkSetting GetSettings(string[] args)
        => new ShrinkSetting(args.Contains("-l"), 
            args.Contains("-a"), 
            args.Contains("-o"), 
            args.Contains("-y"),
            args.Contains("-d")
            );
}