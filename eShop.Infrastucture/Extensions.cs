using eShop.Infrastructure.State;
using eShop.Infrastructure.Storage;

namespace eShop.Infrastructure;

public static class Extensions
{
    public static IHostApplicationBuilder AddInfrastructureLayer(this IHostApplicationBuilder builder)
    {
        builder.AddDependencyInjection();
        builder.AddJwtAuthentication();

        builder.Services.AddAuthorization();
        builder.Services.AddBlazoredLocalStorage();
        builder.Services.AddHttpContextAccessor();
        return builder;
    }

    private static IHostApplicationBuilder AddDependencyInjection(this IHostApplicationBuilder builder)
    {
        builder.Services.AddScoped<AuthenticationStateProvider, ApplicationAuthenticationStateProvider>();

        builder.Services.AddHttpContextAccessor();

        builder.Services.AddHttpClient();
        builder.Services.AddHttpClient<IAuthenticationService, AuthenticationService>();
        builder.Services.AddHttpClient<IProductService, ProductService>();
        builder.Services.AddHttpClient<IBrandService, BrandSevice>();
        builder.Services.AddHttpClient<ICommentService, CommentService>();
        builder.Services.AddHttpClient<IReviewService, ReviewService>();
        builder.Services.AddHttpClient<ICartService, CartService>();
        builder.Services.AddHttpClient<IFavoritesService, FavoritesService>();
        builder.Services.AddHttpClient<IStoreService, StoreService>();
        builder.Services.AddHttpClient<ISellerService, SellerService>();
        builder.Services.AddHttpClient<IProfileService, ProfileService>();

        builder.Services.AddScoped<IHttpClientService, HttpClientService>();
        builder.Services.AddScoped<ITokenProvider, TokenProvider>();
        builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
        builder.Services.AddScoped<IProductService, ProductService>();
        builder.Services.AddScoped<IBrandService, BrandSevice>();
        builder.Services.AddScoped<ILocalStorage, LocalStorage>();
        builder.Services.AddScoped<IUserStorage, UserStorage>();
        builder.Services.AddScoped<IStoreService, StoreService>();
        builder.Services.AddScoped<IReviewService, ReviewService>();
        builder.Services.AddScoped<ICartService, CartService>();
        builder.Services.AddScoped<IFavoritesService, FavoritesService>();
        builder.Services.AddScoped<ISellerService, SellerService>();
        builder.Services.AddScoped<IProfileService, ProfileService>();

        builder.Services.AddScoped<INotificationService, NotificationService>();
        builder.Services.AddSingleton<InputImagesStateContainer>();
        builder.Services.AddSingleton<NotificationsStateContainer>();

        return builder;
    }

}