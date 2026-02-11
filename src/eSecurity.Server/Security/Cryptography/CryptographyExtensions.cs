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
            cfg.OpaqueTokenLength = 32;
            
            cfg.DefaultAccessTokenLifetime = TimeSpan.FromMinutes(10);
            cfg.MinAccessTokenLifetime = TimeSpan.FromMinutes(5);
            cfg.MaxAccessTokenLifetime = TimeSpan.FromMinutes(15);
            
            cfg.DefaultIdTokenLifetime = TimeSpan.FromMinutes(10);
            cfg.MinIdTokenLifetime = TimeSpan.FromMinutes(5);
            cfg.MaxIdTokenLifetime = TimeSpan.FromMinutes(15);
            
            cfg.DefaultLogoutTokenLifetime = TimeSpan.FromMinutes(2);
            cfg.MinLogoutTokenLifetime = TimeSpan.FromSeconds(30);
            cfg.MaxLogoutTokenLifetime = TimeSpan.FromMinutes(5);
            
            cfg.DefaultLoginTokenLifetime = TimeSpan.FromDays(7);
            cfg.MinLoginTokenLifetime = TimeSpan.FromDays(1);
            cfg.MaxLoginTokenLifetime = TimeSpan.FromDays(14);
            
            cfg.DefaultRefreshTokenLifetime = TimeSpan.FromDays(30);
            cfg.MinRefreshTokenLifetime = TimeSpan.FromDays(7);
            cfg.MaxRefreshTokenLifetime = TimeSpan.FromDays(180);
        });

        builder.Services.AddSigning(cfg =>
        {
            cfg.SubjectName = "CN=JwtSigningKey";
            cfg.CertificateLifetime = TimeSpan.FromDays(180);
            cfg.KeyLength = 256;
        });

        builder.Services.AddDataProtection();
        builder.Services.AddTransient<ICodeFactory, CodeFactory>();
        builder.Services.AddTransient<IKeyFactory, RandomKeyFactory>();
        builder.Services.AddScoped<IHasherProvider, HasherProvider>();
        builder.Services.AddKeyedTransient<IHasher, Pbkdf2Hasher>(HashAlgorithm.Pbkdf2);
        builder.Services.AddKeyedTransient<IHasher, Sha256Hasher>(HashAlgorithm.Sha256);
        builder.Services.AddKeyedTransient<IHasher, Sha512Hasher>(HashAlgorithm.Sha512);
    }

    extension(IServiceCollection services)
    {
        private void AddSigning(Action<CertificateOptions> configure)
        {
            services.Configure(configure);
            services.AddScoped<ICertificateProvider, CertificateProvider>();
            services.AddScoped<ICertificateHandler, CertificateHandler>();
        }

        private void AddTokens(Action<TokenConfigurations> configure)
        {
            services.Configure(configure);

            services.AddScoped<ITokenBuilderProvider, TokenBuilderProvider>();
            services.AddScoped<ITokenBuilder<JwtTokenBuildContext, string>, JwtTokenBuilder>();
            services.AddScoped<ITokenBuilder<OpaqueTokenBuildContext, string>, OpaqueTokenBuilder>();
            services.AddScoped<IJwtSigner, JwtSigner>();

            services.AddScoped<ITokenFactoryProvider, TokenFactoryProvider>();
            services.AddKeyedScoped<ITokenFactory, AccessTokenFactory>(TokenType.AccessToken);
            services.AddKeyedScoped<ITokenFactory, RefreshTokenFactory>(TokenType.RefreshToken);
            services.AddKeyedScoped<ITokenFactory, IdTokenFactory>(TokenType.IdToken);
            services.AddKeyedScoped<ITokenFactory, LogoutTokenFactory>(TokenType.LogoutToken);
            services.AddKeyedScoped<ITokenFactory, LoginTokenFactory>(TokenType.LoginToken);
        }
    }
}