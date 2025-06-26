using DatabaseShrinker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Spectre.Console;

var builder = Host.CreateApplicationBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

builder.Services.AddTransient<IRunner, Runner>();
builder.Services.AddSingleton<SqlScripts>();
builder.Services.AddTransient<ISqlConnectorFactory, SqlConnectorFactory>();
builder.Services.AddTransient<Func<string, ISqlConnector>>(
    sp => (string connectionString) => sp.GetRequiredService<ISqlConnectorFactory>().Create(connectionString));

var host = builder.Build();

if (args.Length == 0)
{
    AnsiConsole.WriteLine("No arguments");
    AnsiConsole.WriteLine(DatabaseShrinker.Help.GetHelp());
    return;
}

var shrinkSetting = Help.GetSettings(args);

var RunCommand = (string commandArgument, Action<string, ShrinkSetting> action) =>
{
    var indexOf = args.ToList().IndexOf(commandArgument) + 1;
    if (args.Length > indexOf)
    {
        action(args[indexOf], shrinkSetting);
    }
};

var logger = host.Services.GetRequiredService<ILogger<Program>>();
var sqlConnectorFactory = host.Services.GetRequiredService<Func<string, ISqlConnector>>();
var commands = Help.GetCommands(logger, sqlConnectorFactory);
foreach(var command in commands
    .Where(c => args.Contains(c.CommandArgument)))
{
    RunCommand(command.CommandArgument, command.CommandAction);   
}