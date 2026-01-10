using eSecurity.Server.Common.Storage.Session;

namespace eSecurity.Server.Common.Storage;

public static class StorageExtensions
{
    extension(IHostApplicationBuilder builder)
    {
        public void AddStorage()
        {
            builder.Services.AddScoped<ISessionStorage, SessionStorage>();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(5);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
        }
    }
}