using eSecurity.Security.Cryptography.Codes;
using eSecurity.Security.Cryptography.Hashing;
using eSecurity.Security.Cryptography.Hashing.Hashers;
using eSecurity.Security.Cryptography.Keys;
using eSecurity.Security.Cryptography.Keys.PrivateKey;
using eSecurity.Security.Cryptography.Tokens.Jwt;
using eSystem.Core.Security.Cryptography.Keys;

namespace eSecurity.Security.Cryptography;

public static class CryptographyExtensions
{
    public static void AddCryptography(this IHostApplicationBuilder builder)
    {
        builder.Services.AddTokens();
        builder.Services.AddHashing();
        builder.Services.AddProtection();
        builder.Services.AddKeyManagement();
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

    private static void AddKeyManagement(this IServiceCollection services)
    {
        services.AddScoped<IKeyFactory, RandomKeyFactory>();
        services.AddScoped<IKeyProvider, RsaKeyProvider>();
    }

    private static void AddTokens(this IServiceCollection services)
    {
        services.AddScoped<ITokenFactory, JwtTokenFactory>();
        services.AddScoped<IJwtSigner, JwtSigner>();
    }
}