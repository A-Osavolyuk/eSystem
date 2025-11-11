using Blazored.LocalStorage;
using eSecurity.Client.Common.Storage.Local;

namespace eSecurity.Client.Common.Storage;

public static class StorageExtensions
{
    public static void AddStorage(this IServiceCollection services)
    {
        services.AddScoped<IStorage, LocalStorage>();
        services.AddBlazoredLocalStorage();
        services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(5);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });
    }
}