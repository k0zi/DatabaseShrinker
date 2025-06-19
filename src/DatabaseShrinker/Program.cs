using DatabaseShrinker;
using Spectre.Console;

if (args.Length == 0)
{
    AnsiConsole.WriteLine("No arguments");
    AnsiConsole.WriteLine(DatabaseShrinker.Help.GetHelp());
    return;
}

if (args[0] == "-v")
{
    AnsiConsole.WriteLine("Version: {0}", DatabaseShrinker.Help.GetVersion());
    return;
}

if (args.Length == 1 || args[0] == "-h" || args.Length > 2 ||
    !DatabaseShrinker.Help.GetValidCommands().Contains(args[0]))
{
    AnsiConsole.WriteLine(DatabaseShrinker.Help.GetHelp());
    return;
}

if (args[0] == "-c")
{
    new Runner().RunConnectionString(args[1]);
}

if (args[0] == "-cs")
{
    var strings = new ConnectionStringLoader(args[1]).GetAllConnectionStrings();
    new Runner().RunConnectionStrings(strings);
}