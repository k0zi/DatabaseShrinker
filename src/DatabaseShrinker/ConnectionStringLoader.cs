using Microsoft.Extensions.Configuration;

namespace DatabaseShrinker;

public class ConnectionStringLoader
{
    private readonly IConfiguration _configuration;

    public ConnectionStringLoader(string jsonFilePath = "appsettings.json")
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile(jsonFilePath, optional: false, reloadOnChange: true);
            
        _configuration = builder.Build();
    }
    
    public string GetConnectionString(string name = "DefaultConnection")
    {
        return _configuration.GetConnectionString(name) 
               ?? throw new InvalidOperationException($"Connection string '{name}' not found.");
    }
    
    public string[] GetAllConnectionStrings()
    {
        var connectionStrings = new Dictionary<string, string>();
        var section = _configuration.GetSection("ConnectionStrings");
        
        foreach (var child in section.GetChildren())
        {
            connectionStrings[child.Key] = child.Value ?? string.Empty;
        }
        
        return connectionStrings
            .Select(kv => kv.Value).ToArray();
    }

}