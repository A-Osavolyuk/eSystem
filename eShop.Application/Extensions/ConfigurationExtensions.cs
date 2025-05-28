using Microsoft.Extensions.Configuration;

namespace eShop.Application.Extensions;

public static class ConfigurationExtensions
{
    public static TValue Get<TValue>(this IConfiguration configurationManager, string key)
    {
        var section = configurationManager.GetSection(key);
        
        if (!section.Exists())
        {
            throw new KeyNotFoundException($"Configuration section '{key}' was not found.");
        }

        var value = section.Get<TValue>();

        if (value == null)
        {
            throw new InvalidOperationException($"Configuration value for key '{key}' could not be found or is null.");
        }

        return value;
    }

    public static string GetConnectionString(this IConfiguration configuration, SqlDb type)
    {
        const string path = "Configuration:Storage:Databases:SQL:";
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
        const string path = "Configuration:Storage:Databases:NoSQL:";
        var section = type switch
        {
            NoSqlDb.Mongo => "Mongo",
            NoSqlDb.Cassandra => "Cassandra",
            NoSqlDb.Redis => "Redis",
            NoSqlDb.DynamoDb => "DynamoDb",
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };

        var sb = new StringBuilder(path).Append(section).Append(":ConnectionString");
        var key = sb.ToString();
        var connectionString = configuration.Get<string>(key);
        return connectionString;
    }
}