using eShop.Blazor.Application.Routing;
using eShop.Blazor.Application.State;
using eShop.Blazor.Domain.Interfaces;
using eShop.Blazor.Infrastructure.Implementations;
using eShop.Blazor.Infrastructure.Security;
using eShop.Blazor.Infrastructure.Storage;
using Microsoft.AspNetCore.Builder;

namespace eShop.Blazor.Infrastructure;

public static class Extensions
{
    public static void AddInfrastructureLayer(this WebApplicationBuilder builder)
    {
        builder.AddState();
        builder.AddDependencyInjection();
        builder.AddJwtAuthentication();

        builder.Services.AddBlazoredLocalStorage();
        builder.Services.AddHttpContextAccessor();
    }

    private static void AddDependencyInjection(this WebApplicationBuilder builder)
    {
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddHttpClient();
        
        builder.Services.AddHttpClient<ISecurityService, SecurityService>(ServiceLifetime.Scoped);
        builder.Services.AddHttpClient<IStoreService, StorageService>(ServiceLifetime.Scoped);
        builder.Services.AddHttpClient<ITwoFactorService, TwoFactorService>(ServiceLifetime.Scoped);
        builder.Services.AddHttpClient<IUserService, UserService>(ServiceLifetime.Scoped);
        builder.Services.AddHttpClient<IProvidersService, ProvidersService>(ServiceLifetime.Scoped);
        builder.Services.AddHttpClient<ITypeService, TypeService>(ServiceLifetime.Scoped);
        builder.Services.AddHttpClient<IUnitService, UnitService>(ServiceLifetime.Scoped);
        builder.Services.AddHttpClient<ICategoryService, CategoryService>(ServiceLifetime.Scoped);
        builder.Services.AddHttpClient<IPriceService, PriceService>(ServiceLifetime.Scoped);
        builder.Services.AddHttpClient<ICurrencyService, CurrencyService>(ServiceLifetime.Scoped);
        builder.Services.AddHttpClient<IOAuthService, OAuthService>(ServiceLifetime.Scoped);
        builder.Services.AddHttpClient<IDeviceService, DeviceService>(ServiceLifetime.Scoped);
        builder.Services.AddHttpClient<IPasskeyService, PasskeyService>(ServiceLifetime.Scoped);

        builder.Services.AddScoped<IApiClient, ApiClient>();
        builder.Services.AddScoped<ITokenProvider, TokenProvider>();
        builder.Services.AddScoped<IStorage, LocalStorage>();
        builder.Services.AddScoped<ICookieManager, CookieManager>();
    }

    private static void AddJwtAuthentication(this IHostApplicationBuilder builder)
    {
        builder.Services.AddAuthorization();
        builder.Services.AddAuthentication()
            .AddScheme<JwtAuthenticationOptions, JwtAuthenticationHandler>(
                JwtBearerDefaults.AuthenticationScheme, _ => { });

        builder.Services.AddCascadingAuthenticationState();
        builder.Services.AddScoped<TokenHandler>();
        builder.Services.AddScoped<AuthenticationManager>();
        builder.Services.AddScoped<PasskeyManager>();
        builder.Services.AddScoped<JwtAuthenticationStateProvider>();
        builder.Services.AddScoped<AuthenticationStateProvider, JwtAuthenticationStateProvider>();
    }

    private static void AddState(this IHostApplicationBuilder builder)
    {
        builder.Services.AddScoped<UserState>();
        builder.Services.AddScoped<ProductState>();
    }

    public static void AddRouting(this IHostApplicationBuilder builder, Action<RouteOptions> configureRouter)
    {
        var router = new RouteOptions();
        configureRouter(router);

        builder.Services.AddSingleton(router);
        builder.Services.AddScoped<RouteManager>();
    }
}