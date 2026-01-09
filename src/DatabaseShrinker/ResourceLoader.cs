using System.Reflection;

namespace DatabaseShrinker;

public static class ResourceLoader
{
    private static readonly Assembly Assembly= Assembly.GetExecutingAssembly();
    public static string GetResource(string resourceName)
    {
        using var stream = Assembly.GetManifestResourceStream(resourceName);
        using var reader = new StreamReader(stream!);
        return reader.ReadToEnd();
    }
}