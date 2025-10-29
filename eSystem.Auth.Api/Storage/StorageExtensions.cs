using eSystem.Auth.Api.Storage.Session;

namespace eSystem.Auth.Api.Storage;

public static class StorageExtensions
{
    public static void AddStorage(this IHostApplicationBuilder builder)
    {
        builder.AddSession();
    }
    
    private static void AddSession(this IHostApplicationBuilder builder)
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