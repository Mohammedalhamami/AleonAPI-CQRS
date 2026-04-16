using Npgsql;

namespace AleonAPI.Data;

public static class DataUtility
{

    public static string GetConnectionString(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DbConnection");
        var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");

        if (string.IsNullOrEmpty(databaseUrl))
        {
            Console.WriteLine("DEBUG: DATABASE_URL is null or empty. Falling back to DbConnection.");
            return connectionString!;
        }

        var res = BuildConnectionString(databaseUrl);
        if (string.IsNullOrEmpty(res))
        {
            Console.WriteLine($"DEBUG: BuildConnectionString could not resolve. URL length: {databaseUrl.Length}. Starts with: {(databaseUrl.Length > 5 ? databaseUrl[..5] : databaseUrl)}");
        }
        
        return res ?? connectionString!;
    }


    private static string? BuildConnectionString(string databaseUrl)
    {
        // If the string already looks like a connection string, return it as-is
        if (databaseUrl.Contains("Host=", StringComparison.OrdinalIgnoreCase) || 
            databaseUrl.Contains("Server=", StringComparison.OrdinalIgnoreCase))
        {
            return databaseUrl;
        }

        try
        {
            var databaseUri = new Uri(databaseUrl);
            var userInfo = databaseUri.UserInfo.Split(':');
            var builder = new NpgsqlConnectionStringBuilder
            {
                Host = databaseUri.Host,
                Port = databaseUri.Port,
                Username = userInfo[0],
                Password = userInfo[1],
                Database = databaseUri.LocalPath.TrimStart('/'),
                SslMode = SslMode.Prefer
            };

            return builder.ToString();
        }
        catch (Exception)
        {
            return null;
        }
    }

}