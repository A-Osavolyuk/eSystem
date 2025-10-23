using eShop.Blazor.Server.Application.State;
using eShop.Blazor.Server.Domain.Interfaces;
using eShop.Blazor.Server.Infrastructure.Implementations;
using eShop.Blazor.Server.Infrastructure.Security;
using eShop.Blazor.Server.Infrastructure.Services;
using eShop.Blazor.Server.Infrastructure.Storage;
using eShop.Application.Http;
using Microsoft.AspNetCore.Builder;

namespace eShop.Blazor.Server.Infrastructure;

public static class Extensions
{
    public static void AddInfrastructureLayer(this WebApplicationBuilder builder)
    {
        builder.AddDependencyInjection();
        builder.AddAuthentication();

        builder.Services.AddBlazoredLocalStorage();
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddControllers();
    }

    private static void AddDependencyInjection(this WebApplicationBuilder builder)
    {
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddHttpClient();
        
        builder.Services.AddHttpClient<ISecurityService, SecurityService>(ServiceLifetime.Scoped);
        builder.Services.AddHttpClient<IStoreService, StorageService>(ServiceLifetime.Scoped);
        builder.Services.AddHttpClient<ITwoFactorService, TwoFactorService>(ServiceLifetime.Scoped);
        builder.Services.AddHttpClient<IUserService, UserService>(ServiceLifetime.Scoped);
        builder.Services.AddHttpClient<ITypeService, TypeService>(ServiceLifetime.Scoped);
        builder.Services.AddHttpClient<IUnitService, UnitService>(ServiceLifetime.Scoped);
        builder.Services.AddHttpClient<ICategoryService, CategoryService>(ServiceLifetime.Scoped);
        builder.Services.AddHttpClient<IPriceService, PriceService>(ServiceLifetime.Scoped);
        builder.Services.AddHttpClient<ICurrencyService, CurrencyService>(ServiceLifetime.Scoped);
        builder.Services.AddHttpClient<IOAuthService, OAuthService>(ServiceLifetime.Scoped);
        builder.Services.AddHttpClient<IDeviceService, DeviceService>(ServiceLifetime.Scoped);
        builder.Services.AddHttpClient<IPasskeyService, PasskeyService>(ServiceLifetime.Scoped);
        builder.Services.AddHttpClient<IVerificationService, VerificationService>(ServiceLifetime.Scoped);

        builder.Services.AddScoped<IApiClient, ApiClient>();
        builder.Services.AddScoped<IFetchClient, FetchClient>();
        builder.Services.AddScoped<IStorage, LocalStorage>();
        builder.Services.AddScoped<DownloadManager>();
        builder.Services.AddScoped<ClipboardManager>();
        builder.Services.AddScoped<PrintManager>();
    }
}