using eSystem.Core.Data;

namespace eSecurity.Data;

public static class DataExtensions
{
    public static void AddMsSqlDb(this IHostApplicationBuilder builder)
    {
        builder.AddSqlServerDbContext<AuthDbContext>("auth-db",
            configureDbContextOptions: cfg =>
            {
                cfg.UseAsyncSeeding(async (ctx, _, ct) =>
                {
                    await ctx.SeedAsync<IAssemblyMarker>(ct);
                });
            });
    }
}