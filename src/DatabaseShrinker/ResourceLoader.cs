using System.Reflection;

namespace DatabaseShrinker;

public static class ResourceLoader
{
    private static Assembly assembly= Assembly.GetExecutingAssembly();
    public static string GetResource(string resourceName)
    {
        using var stream = assembly.GetManifestResourceStream(resourceName);
        using var reader = new StreamReader(stream!);
        return reader.ReadToEnd();
    }
}