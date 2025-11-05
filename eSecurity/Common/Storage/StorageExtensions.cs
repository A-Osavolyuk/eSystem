using Blazored.LocalStorage;
using eSecurity.Common.Storage.Local;

namespace eSecurity.Common.Storage;

public static class StorageExtensions
{
    public static void AddStorage(this IHostApplicationBuilder builder)
    {
        builder.Services.AddScoped<IStorage, LocalStorage>();
        builder.Services.AddBlazoredLocalStorage();
    }
}