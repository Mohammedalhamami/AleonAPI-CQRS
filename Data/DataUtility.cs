namespace AleonAPI.Data;

public static class DataUtility
{

    public static string GetConnectionString(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        return connectionString!;
    }
    
}