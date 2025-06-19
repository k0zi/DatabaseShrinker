using System.Runtime.CompilerServices;

namespace DatabaseShrinker;

public static class Help
{
    public static string GetHelp() => @"Database Shrinker
Manual 
-c ""connection string"" : a single connection string to shrink
-cs ""path/of/a/connections.json"" : a json file with connection string(s) to shrink
-v : show version
-h : shows this help";

    public static string GetVersion()
    {
        System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
        System.Diagnostics.FileVersionInfo fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
        return fvi.FileVersion;
    }
    
    public static string[] GetValidCommands() => ["-c", "-cs", "-v", "-h"];
}