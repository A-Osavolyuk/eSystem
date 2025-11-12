using eSystem.Core.Data;

namespace eSecurity.Server.Data;

public static class DataExtensions
{
    extension(IHostApplicationBuilder builder)
    {
        public void AddMsSqlDb()
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
}