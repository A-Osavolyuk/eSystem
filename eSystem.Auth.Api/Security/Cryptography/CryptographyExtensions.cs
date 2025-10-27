using eSystem.Auth.Api.Security.Cryptography.Codes;
using eSystem.Auth.Api.Security.Cryptography.Hashing;
using eSystem.Auth.Api.Security.Cryptography.Hashing.Hashers;
using eSystem.Auth.Api.Security.Cryptography.Keys;
using eSystem.Auth.Api.Security.Cryptography.Protection;
using eSystem.Auth.Api.Security.Cryptography.Protection.Protectors;
using eSystem.Core.Security.Cryptography.Keys;
using eSystem.Core.Security.Cryptography.Protection;

namespace eSystem.Auth.Api.Security.Cryptography;

public static class CryptographyExtensions
{
    public static void AddCryptography(this IHostApplicationBuilder builder)
    {
        builder.Services.AddHashing();
        builder.Services.AddProtection();
        builder.Services.AddScoped<IKeyFactory, RandomKeyFactory>();
        builder.Services.AddScoped<ICodeFactory, CodeFactory>();
    }
    private static void AddProtection(this IServiceCollection services)
    {
        services.AddDataProtection();
        services.AddScoped<IProtectorFactory, ProtectorFactory>();
        services.AddKeyedScoped<IProtector, CodeProtector>(ProtectionPurposes.RecoveryCode);
        services.AddKeyedScoped<IProtector, SecretProtector>(ProtectionPurposes.Session);
        services.AddKeyedScoped<IProtector, TokenProtector>(ProtectionPurposes.RefreshToken);
    }

    private static void AddHashing(this IServiceCollection services)
    {
        services.AddScoped<IHasherFactory, HasherFactory>();
        services.AddKeyedScoped<Hasher, Pbkdf2Hasher>(HashAlgorithm.Pbkdf2);
    }
}