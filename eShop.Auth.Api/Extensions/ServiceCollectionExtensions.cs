using eShop.Application.Security.Authorization.Requirements;
using eShop.Auth.Api.Security.Authentication;
using eShop.Auth.Api.Security.Credentials.PublicKey;
using eShop.Auth.Api.Security.Cryptography;
using eShop.Auth.Api.Security.Hashing;
using eShop.Auth.Api.Security.Protection;
using eShop.Auth.Api.Security.TwoFactor.Authenticator;

namespace eShop.Auth.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddIdentity(this IServiceCollection services,
        Action<IdentityOptions> configureOptions)
    {
        var options = new IdentityOptions();
        configureOptions(options);

        services.AddSingleton(options);
    }

    public static void AddVerification(this IServiceCollection services, Action<VerificationOptions> configure)
    {
        var options = new VerificationOptions();
        configure(options);

        services.AddSingleton(options);
    }

    public static void Add2FA(this IServiceCollection services, Action<TwoFactorOptions> configure)
    {
        var options = new TwoFactorOptions();
        configure(options);

        services.AddSingleton(options);
        services.AddScoped<IQrCodeFactory, QrCodeFactory>();
    }

    public static void AddSignInStrategies(this IServiceCollection services)
    {
        services.AddScoped<ISignInResolver, SignInResolver>();
        services.AddKeyedScoped<SignInStrategy, PasswordSignInStrategy>(SignInType.Password);
        services.AddKeyedScoped<SignInStrategy, PasskeySignInStrategy>(SignInType.Passkey);
        services.AddKeyedScoped<SignInStrategy, AuthenticatorSignInStrategy>(SignInType.AuthenticatorApp);
        services.AddKeyedScoped<SignInStrategy, LinkedAccountSignInStrategy>(SignInType.LinkedAccount);
    }

    public static void AddProtection(this IServiceCollection services)
    {
        services.AddDataProtection();
        services.AddScoped<IProtectorFactory, ProtectorFactory>();
        services.AddKeyedScoped<Protector, CodeProtector>(ProtectorType.Code);
        services.AddKeyedScoped<Protector, SecretProtector>(ProtectorType.Secret);
    }

    public static void AddHashing(this IServiceCollection services)
    {
        services.AddScoped<IHasherFactory, HasherFactory>();
        services.AddKeyedScoped<Hasher, Pbkdf2Hasher>(HashAlgorithm.Pbkdf2);
    }

    public static void AddCryptography(this IServiceCollection services)
    {
        services.AddScoped<IKeyFactory, KeyFactory>();
        services.AddScoped<ICodeFactory, CodeFactory>();
    }

    public static void AddCredentials(this IServiceCollection services)
    {
        services.AddScoped<IChallengeFactory, ChallengeFactory>();
        services.AddScoped<ICredentialFactory, CredentialFactory>();
    }

    public static void AddAuthorization(this IServiceCollection services)
    {
        services.AddAuthorizationBuilder()
            .AddPolicy("DeleteAccountPolicy", policy => policy.Requirements.Add(new PermissionRequirement("DELETE_ACCOUNT")))
            .AddPolicy("CreateAccountPolicy", policy => policy.Requirements.Add(new PermissionRequirement("CREATE_ACCOUNT")))
            .AddPolicy("UpdateAccountPolicy", policy => policy.Requirements.Add(new PermissionRequirement("UPDATE_ACCOUNT")))
            .AddPolicy("ReadAccountPolicy", policy => policy.Requirements.Add(new PermissionRequirement("READ_ACCOUNT")))
            .AddPolicy("DeleteUserPolicy", policy => policy.Requirements.Add(new PermissionRequirement("DELETE_USER")))
            .AddPolicy("CreateUserPolicy", policy => policy.Requirements.Add(new PermissionRequirement("CREATE_USER")))
            .AddPolicy("UpdateUserPolicy", policy => policy.Requirements.Add(new PermissionRequirement("UPDATE_USER")))
            .AddPolicy("ReadUserPolicy", policy => policy.Requirements.Add(new PermissionRequirement("READ_USER")))
            .AddPolicy("LockoutUserPolicy", policy => policy.Requirements.Add(new PermissionRequirement("LOCKOUT_USER")))
            .AddPolicy("UnlockUserPolicy", policy => policy.Requirements.Add(new PermissionRequirement("UNLOCK_USER")))
            .AddPolicy("DeleteRolePolicy", policy => policy.Requirements.Add(new PermissionRequirement("DELETE_ROLE")))
            .AddPolicy("CreateRolePolicy", policy => policy.Requirements.Add(new PermissionRequirement("CREATE_ROLE")))
            .AddPolicy("UpdateRolePolicy", policy => policy.Requirements.Add(new PermissionRequirement("UPDATE_ROLE")))
            .AddPolicy("ReadRolePolicy", policy => policy.Requirements.Add(new PermissionRequirement("READ_ROLE")))
            .AddPolicy("AssignRolePolicy", policy => policy.Requirements.Add(new PermissionRequirement("ASSIGN_ROLE")))
            .AddPolicy("UnassignRolePolicy", policy => policy.Requirements.Add(new PermissionRequirement("UNASSIGN_ROLE")))
            .AddPolicy("DeletePermissionPolicy", policy => policy.Requirements.Add(new PermissionRequirement("DELETE_PERMISSION")))
            .AddPolicy("CreatePermissionPolicy", policy => policy.Requirements.Add(new PermissionRequirement("CREATE_PERMISSION")))
            .AddPolicy("UpdatePermissionPolicy", policy => policy.Requirements.Add(new PermissionRequirement("UPDATE_PERMISSION")))
            .AddPolicy("ReadPermissionPolicy", policy => policy.Requirements.Add(new PermissionRequirement("READ_PERMISSIONS")))
            .AddPolicy("GrantPermissionPolicy", policy => policy.Requirements.Add(new PermissionRequirement("GRANT_PERMISSIONS")))
            .AddPolicy("RevokePermissionPolicy", policy => policy.Requirements.Add(new PermissionRequirement("REVOKE_PERMISSIONS")));
    }
}