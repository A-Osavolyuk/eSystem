using eSecurity.Server.Data.Interceptors;
using eSystem.Core.Data;

namespace eSecurity.Server.Data;

public static class DataExtensions
{
    extension(IHostApplicationBuilder builder)
    {
        public void AddMsSqlDb()
        {
            builder.Services.AddSingleton<AuditInterceptor>();
            builder.Services.AddDbContext<AuthDbContext>((sp, options) =>
            {
                var cfg = sp.GetRequiredService<IConfiguration>();
                options.UseSqlServer(cfg.GetConnectionString("auth-db"));
                options.AddInterceptors(sp.GetRequiredService<AuditInterceptor>());
                options.UseAsyncSeeding(async (ctx, _, ct) =>
                {
                    await ctx.SeedAsync<IAssemblyMarker>(ct);
                });
            });
        }
    }
}