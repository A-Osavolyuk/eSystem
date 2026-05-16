using Microsoft.EntityFrameworkCore;

namespace eSecurity.Client.BFF.Data;

public static class DbExtensions
{
    public static void AddSqlDb(this IHostApplicationBuilder builder, string connectionName)
    {
        builder.Services.AddDbContext<AppDbContext>((sp, options) =>
        {
            var configurations = sp.GetRequiredService<IConfiguration>();
            options.UseNpgsql(configurations.GetConnectionString(connectionName));
        });
    }
}