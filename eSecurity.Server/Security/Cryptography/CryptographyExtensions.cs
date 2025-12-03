using eSecurity.Server.Security.Cryptography.Codes;
using eSecurity.Server.Security.Cryptography.Hashing;
using eSecurity.Server.Security.Cryptography.Hashing.Hashers;
using eSecurity.Server.Security.Cryptography.Keys;
using eSecurity.Server.Security.Cryptography.Signing;
using eSecurity.Server.Security.Cryptography.Signing.Certificates;
using eSecurity.Server.Security.Cryptography.Tokens;

namespace eSecurity.Server.Security.Cryptography;

public static class CryptographyExtensions
{
    public static void AddCryptography(this IHostApplicationBuilder builder)
    {
        builder.Services.AddTokens(cfg =>
        {
            cfg.Issuer = "https://localhost:6201";
            cfg.AccessTokenLifetime = TimeSpan.FromMinutes(10);
            cfg.IdTokenLifetime = TimeSpan.FromMinutes(10);
            cfg.OpaqueTokenLifetime = TimeSpan.FromMinutes(10);
            cfg.OpaqueTokenLength = 20;
            cfg.RefreshTokenLength = 20;
        });

        builder.Services.AddSigning(cfg =>
        {
            cfg.SubjectName = "CN=JwtSigningKey";
            cfg.CertificateLifetime = TimeSpan.FromDays(180);
            cfg.KeyLength = 256;
        });

        builder.Services.AddDataProtection();
        builder.Services.AddScoped<IHasherFactory, HasherFactory>();
        builder.Services.AddKeyedTransient<Hasher, Pbkdf2Hasher>(HashAlgorithm.Pbkdf2);
        builder.Services.AddTransient<ICodeFactory, CodeFactory>();
        builder.Services.AddTransient<IKeyFactory, RandomKeyFactory>();
    }

    extension(IServiceCollection services)
    {
        private void AddSigning(Action<CertificateOptions> configure)
        {
            services.Configure(configure);
            services.AddScoped<ICertificateProvider, CertificateProvider>();
            services.AddScoped<ICertificateHandler, CertificateHandler>();
        }

        private void AddTokens(Action<TokenOptions> configure)
        {
            services.Configure(configure);

            services.AddScoped<ITokenFactoryProvider, TokenFactoryProvider>();
            services.AddScoped<ITokenFactory<JwtTokenContext, string>, JwtTokenFactory>();
            services.AddScoped<ITokenFactory<OpaqueTokenContext, string>, OpaqueTokenFactory>();
            services.AddScoped<IJwtSigner, JwtSigner>();
        }
    }
}