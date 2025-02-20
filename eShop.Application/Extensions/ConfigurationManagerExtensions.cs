using Microsoft.Extensions.Configuration;

namespace eShop.Application.Extensions;

public static class ConfigurationManagerExtensions
{
    public static TValue Get<TValue>(this IConfiguration configurationManager, string key)
    {
        var section = configurationManager.GetSection(key);
        var value = section.Get<TValue>()!;
        return value;
    }

    public static string GetConnectionString(this IConfiguration configuration, SqlDb type)
    {
        const string path = "Configuration:Storage:Database:SQL:";
        var section = type switch
        {
            SqlDb.SqlServer => "MSSQL",
            SqlDb.Oracle => "ORACLE",
            SqlDb.MySql => "MySQL",
            SqlDb.Postgres => "PostgreSQL",
            SqlDb.Sqlite => "SQLite",
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };

        var sb = new StringBuilder(path).Append(section).Append(":ConnectionString");
        var key = sb.ToString();
        var connectionString = configuration.Get<string>(key);
        return connectionString;
    }
    
    public static string GetConnectionString(this IConfiguration configuration, NoSqlDb type)
    {
        const string path = "Configuration:Storage:Database:SQL:";
        var section = type switch
        {
            NoSqlDb.Mongo => "Mongo",
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };

        var sb = new StringBuilder(path).Append(section).Append(":ConnectionString");
        var key = sb.ToString();
        var connectionString = configuration.Get<string>(key);
        return connectionString;
    }
}