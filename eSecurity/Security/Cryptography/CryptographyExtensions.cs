using eSecurity.Security.Cryptography.Codes;
using eSecurity.Security.Cryptography.Hashing;
using eSecurity.Security.Cryptography.Hashing.Hashers;
using eSecurity.Security.Cryptography.Keys;
using eSecurity.Security.Cryptography.Signing.Certificates;
using eSecurity.Security.Cryptography.Tokens.Jwt;

namespace eSecurity.Security.Cryptography;

public static class CryptographyExtensions
{
    public static void AddCryptography(this IHostApplicationBuilder builder)
    {
        builder.Services.AddJwt(cfg =>
        {
            cfg.Issuer = "eSecurity";
            cfg.AccessTokenLifetime = TimeSpan.FromMinutes(10);
            cfg.IdTokenLifetime = TimeSpan.FromMinutes(10);
        });
        
        builder.Services.AddSigning(cfg =>
        {
            cfg.SubjectName = "CN=JwtSigningKey";
            cfg.CertificateLifetime = TimeSpan.FromDays(180);
            cfg.KeyLength = 256;
        });

        builder.Services.AddKeys();
        builder.Services.AddHashing();
        builder.Services.AddProtection();
        builder.Services.AddScoped<ICodeFactory, CodeFactory>();
    }
    private static void AddProtection(this IServiceCollection services)
    {
        services.AddDataProtection();
    }

    private static void AddHashing(this IServiceCollection services)
    {
        services.AddScoped<IHasherFactory, HasherFactory>();
        services.AddKeyedScoped<Hasher, Pbkdf2Hasher>(HashAlgorithm.Pbkdf2);
    }

    private static void AddKeys(this IServiceCollection services)
    {
        services.AddScoped<IKeyFactory, RandomKeyFactory>();
    }

    private static void AddSigning(this IServiceCollection services, Action<CertificateOptions> configure)
    {
        services.Configure(configure);
        services.AddScoped<ICertificateProvider, CertificateProvider>();
        services.AddScoped<ICertificateHandler, CertificateHandler>();
    }
    
    private static void AddJwt(this IServiceCollection services, Action<TokenOptions> configure)
    {
        services.Configure(configure);
        
        services.AddScoped<ITokenFactory, JwtTokenFactory>();
        services.AddScoped<IJwtSigner, JwtSigner>();
    }
}