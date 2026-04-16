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
            return connectionString!;
        }

        return BuildConnectionString(databaseUrl) ?? connectionString!;
    }


    private static string? BuildConnectionString(string databaseUrl)
    {
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