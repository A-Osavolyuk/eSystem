using Blazored.LocalStorage;
using eSecurity.Client.Common.Storage.Local;

namespace eSecurity.Client.Common.Storage;

public static class StorageExtensions
{
    public static void AddStorage(this IServiceCollection services)
    {
        services.AddScoped<IStorage, LocalStorage>();
        services.AddBlazoredLocalStorage();
    }
}