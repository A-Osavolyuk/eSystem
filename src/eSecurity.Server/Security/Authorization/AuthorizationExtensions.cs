using eSecurity.Server.Security.Authentication.OpenIdConnect.Ciba;
using eSecurity.Server.Security.Authorization.Access;
using eSecurity.Server.Security.Authorization.Constants;
using eSecurity.Server.Security.Authorization.Devices;
using eSecurity.Server.Security.Authorization.OAuth.Consents;
using eSecurity.Server.Security.Authorization.OAuth.LinkedAccount;
using eSecurity.Server.Security.Authorization.OAuth.Session;
using eSecurity.Server.Security.Authorization.OAuth.Token;
using eSecurity.Server.Security.Authorization.OAuth.Token.DeviceCode;
using eSecurity.Server.Security.Authorization.OAuth.Token.Validation;
using eSecurity.Server.Security.Authorization.Roles;
using eSystem.Core.Security.Authentication.Schemes;
using eSystem.Core.Security.Authorization.OAuth.Constants;
using eSystem.Core.Security.Authorization.OAuth.Token.Validation;

namespace eSecurity.Server.Security.Authorization;

public static class AuthorizationExtensions
{
    public static void AddAuthorization(this IHostApplicationBuilder builder)
    {
        builder.Services.AddTokenFlow();
        builder.Services.AddRoleManagement();
        builder.Services.AddAccessManagement();
        builder.Services.AddDeviceManagement();
        builder.Services.AddDeviceAuthorization(options =>
        {
            options.DeviceCodeLenght = 32;
            options.UserCodeLenght = 8;
            options.Interval = 5;
            options.Timestamp = TimeSpan.FromSeconds(3600);
            options.VerificationUri = "https://localhost:6501/device/activate";
        });

        builder.Services.AddScoped<ICibaRequestManager, CibaRequestManager>();
        builder.Services.AddScoped<IOAuthSessionManager, OAuthSessionManager>();
        builder.Services.AddScoped<IConsentManager, ConsentManager>();
        builder.Services.AddScoped<ILinkedAccountManager, LinkedAccountManager>();
        builder.Services.AddScoped<ITokenValidationProvider, TokenValidationProvider>();
        builder.Services.AddScoped<IJwtTokenValidationProvider, JwtTokenValidationProvider>();
        builder.Services.AddKeyedScoped<ITokenValidator, OpaqueTokenValidator>(TokenKind.Opaque);
        builder.Services.AddKeyedScoped<ITokenValidator, JwtTokenValidator>(TokenKind.Jwt);
        builder.Services.AddKeyedScoped<IJwtTokenValidator, IdTokenValidator>(JwtTokenTypes.IdToken);
        builder.Services.AddKeyedScoped<IJwtTokenValidator, AccessTokenValidator>(JwtTokenTypes.AccessToken);
        builder.Services.AddKeyedScoped<IJwtTokenValidator, GenericJwtTokenValidator>(JwtTokenTypes.Generic);

        builder.Services.AddAuthorizationBuilder()
            .AddPolicy(AuthorizationPolicies.BasicAuthorization, policy =>
            {
                policy.AddAuthenticationSchemes(
                    ClientSecretBasicAuthenticationDefaults.AuthenticationScheme,
                    ClientSecretPostAuthenticationDefaults.AuthenticationScheme);
                policy.RequireAuthenticatedUser();
            })
            .AddPolicy(AuthorizationPolicies.TokenAuthorization, policy =>
            {
                policy.AddAuthenticationSchemes(
                    ClientSecretBasicAuthenticationDefaults.AuthenticationScheme,
                    ClientSecretPostAuthenticationDefaults.AuthenticationScheme);
                policy.RequireAuthenticatedUser();
            });
    }
}