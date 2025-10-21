using eShop.Auth.Api.Security.Cryptography.Codes;
using eShop.Auth.Api.Security.Cryptography.Hashing;
using eShop.Auth.Api.Security.Cryptography.Hashing.Hashers;
using eShop.Auth.Api.Security.Cryptography.Keys;
using eShop.Auth.Api.Security.Cryptography.Protection;
using eShop.Auth.Api.Security.Cryptography.Protection.Protectors;

namespace eShop.Auth.Api.Security.Cryptography;

public static class CryptographyExtensions
{
    public static void AddCryptography(this IHostApplicationBuilder builder)
    {
        builder.Services.AddHashing();
        builder.Services.AddProtection();
        builder.Services.AddScoped<IKeyFactory, KeyFactory>();
        builder.Services.AddScoped<ICodeFactory, CodeFactory>();
    }
    private static void AddProtection(this IServiceCollection services)
    {
        services.AddDataProtection();
        services.AddScoped<IProtectorFactory, ProtectorFactory>();
        services.AddKeyedScoped<Protector, CodeProtector>(ProtectorType.Code);
        services.AddKeyedScoped<Protector, SecretProtector>(ProtectorType.Secret);
    }

    private static void AddHashing(this IServiceCollection services)
    {
        services.AddScoped<IHasherFactory, HasherFactory>();
        services.AddKeyedScoped<Hasher, Pbkdf2Hasher>(HashAlgorithm.Pbkdf2);
    }
}