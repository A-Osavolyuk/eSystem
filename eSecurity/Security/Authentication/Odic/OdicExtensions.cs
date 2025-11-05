using eSecurity.Security.Authentication.Odic.Client;
using eSecurity.Security.Authentication.Odic.Code;
using eSecurity.Security.Authentication.Odic.Pkce;
using eSecurity.Security.Authentication.Odic.Session;
using eSecurity.Security.Authentication.Odic.Token;
using eSecurity.Security.Authentication.Odic.Token.Strategies;
using eSecurity.Security.Cryptography.Tokens.Jwt;
using eSecurity.Security.Identity.Claims;
using eSystem.Core.Security.Authentication.Jwt;
using eSystem.Core.Security.Authentication.Odic.Constants;

namespace eSecurity.Security.Authentication.Odic;

public static class OdicExtensions
{
    public static void AddOdic(this IServiceCollection services)
    {
        services.AddSession(cfg =>
        {
            cfg.Timestamp = TimeSpan.FromDays(30);
        });
        
        services.AddScoped<IClientManager, ClientManager>();
        services.AddScoped<IAuthorizationCodeManager, AuthorizationCodeManager>();
        services.AddScoped<IPkceHandler, PkceHandler>();
        services.AddScoped<ITokenStrategyResolver, TokenStrategyResolver>();
        services.AddKeyedScoped<TokenStrategy, AuthorizationCodeStrategy>(GrantTypes.AuthorizationCode);
        services.AddKeyedScoped<TokenStrategy, RefreshTokenStrategy>(GrantTypes.RefreshToken);
        
        var configuration = services.BuildServiceProvider().GetRequiredService<IConfiguration>();
        
        services.AddScoped<ITokenFactory, JwtTokenFactory>();
        services.AddScoped<ITokenManager, TokenManager>();
        services.AddSingleton<IClaimBuilderFactory, ClaimBuilderFactory>();
        
        services.Configure<JwtOptions>(configuration.GetSection("Jwt"));
    }
}