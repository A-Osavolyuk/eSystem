namespace eSecurity.Server.Security.Authentication.OpenIdConnect.BackchannelAuthentication;

public static class BackchannelAuthenticationExtensions
{
    public static void AddBackchannelAuthentication(this IServiceCollection services, 
        Action<BackchannelAuthenticationOptions> configureOptions)
    {
        services.Configure(configureOptions);
        services.AddScoped<IUserResolverProvider, UserResolverProvider>();
        services.AddKeyedScoped<IUserResolver, LoginHintUserResolver>(UserHint.LoginHint);
        services.AddKeyedScoped<IUserResolver, LoginTokenHintUserResolver>(UserHint.LoginTokenHint);
        services.AddKeyedScoped<IUserResolver, IdTokenHintUserResolver>(UserHint.IdTokenHint);
    }
}