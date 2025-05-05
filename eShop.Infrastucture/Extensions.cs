using eShop.Infrastructure.State;
using eShop.Infrastructure.Storage;
using Microsoft.IdentityModel.Tokens;

namespace eShop.Infrastructure;

public static class Extensions
{
    public static void AddInfrastructureLayer(this IHostApplicationBuilder builder)
    {
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
        builder.Services.AddHttpClient<IProductService, ProductService>(ServiceLifetime.Scoped);
        builder.Services.AddHttpClient<IBrandService, BrandService>(ServiceLifetime.Scoped);
        builder.Services.AddHttpClient<ICommentService, CommentService>(ServiceLifetime.Scoped);
        builder.Services.AddHttpClient<IReviewService, ReviewService>(ServiceLifetime.Scoped);
        builder.Services.AddHttpClient<ICartService, CartService>(ServiceLifetime.Scoped);
        builder.Services.AddHttpClient<IFavoritesService, FavoritesService>(ServiceLifetime.Scoped);
        builder.Services.AddHttpClient<IStoreService, StoreService>(ServiceLifetime.Scoped);
        builder.Services.AddHttpClient<ISellerService, SellerService>(ServiceLifetime.Scoped);
        builder.Services.AddHttpClient<IProfileService, ProfileService>(ServiceLifetime.Scoped);

        builder.Services.AddScoped<IHttpClientService, HttpClientService>();
        builder.Services.AddScoped<ITokenProvider, TokenProvider>();
        builder.Services.AddScoped<IUserStorage, UserStorage>();
        builder.Services.AddScoped<IStorage, LocalStorage>();
        builder.Services.AddScoped<ICookieManager, CookieManager>();

        builder.Services.AddScoped<INotificationService, NotificationService>();
        builder.Services.AddSingleton<InputImagesStateContainer>();
        builder.Services.AddSingleton<NotificationsStateContainer>();
    }
    
    private static void AddJwtAuthentication(this IHostApplicationBuilder builder)
    {
        builder.Services.AddAuthorization();
        builder.Services.AddAuthentication()
            .AddScheme<JwtAuthenticationOptions, JwtAuthenticationHandler>(
                JwtBearerDefaults.AuthenticationScheme, options => { });
        
        builder.Services.AddCascadingAuthenticationState();
        builder.Services.AddScoped<JwtAuthenticationStateProvider>();
        builder.Services.AddScoped<AuthenticationStateProvider, JwtAuthenticationStateProvider>();
    }

}