using eShop.Infrastructure.Security;
using eShop.Infrastructure.State;
using eShop.Infrastructure.Storage;
using AuthenticationManager = eShop.Infrastructure.Security.AuthenticationManager;

namespace eShop.Infrastructure;

public static class Extensions
{
    public static void AddInfrastructureLayer(this IHostApplicationBuilder builder)
    {
        builder.AddState();
        builder.AddDependencyInjection();
        builder.AddJwtAuthentication();
        
        builder.Services.AddBlazoredLocalStorage();
        builder.Services.AddHttpContextAccessor();
    }

    private static void AddDependencyInjection(this IHostApplicationBuilder builder)
    {
        builder.Services.AddHttpContextAccessor();

        builder.Services.AddHttpClient();
        builder.Services.AddHttpClient<ISecurityService, SecurityService>(ServiceLifetime.Scoped);
        builder.Services.AddHttpClient<IStoreService, StorageService>(ServiceLifetime.Scoped);
        builder.Services.AddHttpClient<ITwoFactorService, TwoFactorService>(ServiceLifetime.Scoped);
        builder.Services.AddHttpClient<IUsersService, UsersService>(ServiceLifetime.Scoped);
        builder.Services.AddHttpClient<IProvidersService, ProvidersService>(ServiceLifetime.Scoped);
        builder.Services.AddHttpClient<ITypeService, TypeService>(ServiceLifetime.Scoped);
        builder.Services.AddHttpClient<IUnitService, UnitService>(ServiceLifetime.Scoped);
        builder.Services.AddHttpClient<ICategoryService, CategoryService>(ServiceLifetime.Scoped);
        builder.Services.AddHttpClient<IPriceService, PriceService>(ServiceLifetime.Scoped);

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
                JwtBearerDefaults.AuthenticationScheme, options => { });
        
        builder.Services.AddCascadingAuthenticationState();
        builder.Services.AddScoped<TokenHandler>();
        builder.Services.AddScoped<AuthenticationManager>();
        builder.Services.AddScoped<JwtAuthenticationStateProvider>();
        builder.Services.AddScoped<AuthenticationStateProvider, JwtAuthenticationStateProvider>();
    }

    private static void AddState(this IHostApplicationBuilder builder)
    {
        builder.Services.AddScoped<UserState>();
        builder.Services.AddScoped<ProductState>();
    }

}