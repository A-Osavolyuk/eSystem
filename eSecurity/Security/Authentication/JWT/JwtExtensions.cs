using eSecurity.Security.Authentication.JWT.Enrichers;
using eSecurity.Security.Authentication.JWT.Factories;
using eSecurity.Security.Authentication.JWT.Management;
using eSecurity.Security.Authentication.JWT.Signing;
using eSystem.Core.Security.Authentication.JWT;
using eSystem.Core.Security.Cryptography.Tokens;

namespace eSecurity.Security.Authentication.JWT;

public static class JwtExtensions
{
    public static void AddJwt(this IServiceCollection services)
    {
        var configuration = services.BuildServiceProvider().GetRequiredService<IConfiguration>();
        
        services.AddTransient<IClaimEnricher, EmailClaimEnricher>();
        services.AddTransient<IClaimEnricher, PhoneClaimEnricher>();
        services.AddTransient<IClaimEnricher, ProfileClaimEnricher>();
        services.AddTransient<IClaimEnricher, AddressClaimEnricher>();
        services.AddTransient<IJwtSigner, JwtSigner>();
        
        services.AddKeyedScoped<ITokenFactory, AccessTokenFactory>(JwtTokenType.AccessToken);
        services.AddKeyedScoped<ITokenFactory, IdTokenFactory>(JwtTokenType.IdToken);
        services.AddScoped<ITokenFactoryResolver, TokenFactoryResolver>();
        services.AddScoped<ITokenManager, TokenManager>();
        
        services.Configure<JwtOptions>(configuration.GetSection("Jwt"));
    }
}