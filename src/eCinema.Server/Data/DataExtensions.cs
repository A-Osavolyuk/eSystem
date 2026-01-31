using Microsoft.EntityFrameworkCore;

namespace eCinema.Server.Data;

public static class DataExtensions
{
    extension(IHostApplicationBuilder builder)
    {
        public void AddDatabase()
        {
            builder.Services.AddDbContext<AppDbContext>((sp, options) =>
            {
                var cfg = sp.GetRequiredService<IConfiguration>();
                options.UseNpgsql(cfg.GetConnectionString("e-cinema-db"));
            });
        }
    }
}