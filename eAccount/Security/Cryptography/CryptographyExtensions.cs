using eAccount.Security.Cryptography.Keys;
using eAccount.Security.Cryptography.Protection;
using eSystem.Core.Security.Cryptography.Keys;
using eSystem.Core.Security.Cryptography.Protection;

namespace eAccount.Security.Cryptography;

public static class CryptographyExtensions
{
    public static void AddCryptography(this IHostApplicationBuilder builder)
    {
        builder.Services.AddDataProtection();
        builder.Services.AddScoped<IProtectorFactory, ProtectorFactory>();
        builder.Services.AddKeyedScoped<IProtector, SessionProtector>(ProtectionPurposes.Session);
        builder.Services.AddScoped<IKeyFactory, RandomKeyFactory>();
    }
}