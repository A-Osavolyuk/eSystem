using eAccount.Infrastructure.Security;
using eAccount.Infrastructure.Implementations;
using eAccount.Infrastructure.Services;
using eAccount.Infrastructure.Storage;
using Microsoft.AspNetCore.Builder;

namespace eAccount.Infrastructure;

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
        
        builder.Services.AddScoped<ISecurityService, SecurityService>();
        builder.Services.AddScoped<IStoreService, StorageService>();
        builder.Services.AddScoped<ITwoFactorService, TwoFactorService>();
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<ITypeService, TypeService>();
        builder.Services.AddScoped<IUnitService, UnitService>();
        builder.Services.AddScoped<ICategoryService, CategoryService>();
        builder.Services.AddScoped<IPriceService, PriceService>();
        builder.Services.AddScoped<ICurrencyService, CurrencyService>();
        builder.Services.AddScoped<IOAuthService, OAuthService>();
        builder.Services.AddScoped<IDeviceService, DeviceService>();
        builder.Services.AddScoped<IPasskeyService, PasskeyService>();
        builder.Services.AddScoped<IVerificationService, VerificationService>();
        builder.Services.AddScoped<ISsoService, SsoService>();

        builder.Services.AddScoped<IApiClient, ApiClient>();
        builder.Services.AddScoped<IFetchClient, FetchClient>();
        builder.Services.AddScoped<IStorage, LocalStorage>();
        builder.Services.AddScoped<DownloadManager>();
        builder.Services.AddScoped<ClipboardManager>();
        builder.Services.AddScoped<PrintManager>();
    }
}