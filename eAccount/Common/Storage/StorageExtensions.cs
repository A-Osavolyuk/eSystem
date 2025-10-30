using Blazored.LocalStorage;
using eAccount.Common.Storage.External;
using eAccount.Common.Storage.Local;

namespace eAccount.Common.Storage;

public static class StorageExtensions
{
    public static void AddStorage(this IHostApplicationBuilder builder)
    {
        builder.Services.AddScoped<IStorage, LocalStorage>();
        builder.Services.AddScoped<IStorageService, StorageService>();
        builder.Services.AddBlazoredLocalStorage();
    }
}