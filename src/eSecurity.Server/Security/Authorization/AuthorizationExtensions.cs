using eSecurity.Server.Security.Authorization.Access;
using eSecurity.Server.Security.Authorization.Constants;
using eSecurity.Server.Security.Authorization.Devices;
using eSecurity.Server.Security.Authorization.OAuth;
using eSecurity.Server.Security.Authorization.OAuth.Consents;
using eSecurity.Server.Security.Authorization.OAuth.Protocol;
using eSecurity.Server.Security.Authorization.OAuth.Token;
using eSecurity.Server.Security.Authorization.OAuth.Token.AuthorizationCode;
using eSecurity.Server.Security.Authorization.OAuth.Token.ClientCredentials;
using eSecurity.Server.Security.Authorization.OAuth.Token.DeviceCode;
using eSecurity.Server.Security.Authorization.OAuth.Token.RefreshToken;
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
        builder.Services.AddRoleManagement();
        builder.Services.AddAccessManagement();
        builder.Services.AddDeviceManagement();
        builder.Services.AddOAuthAuthorization();
        builder.Services.AddDeviceAuthorization(options =>
        {
            options.DeviceCodeLenght = 32;
            options.UserCodeLenght = 8;
            options.Interval = 5;
            options.Timestamp = TimeSpan.FromSeconds(3600);
            options.VerificationUri = "https://localhost:6501/device/activate";
        });

        builder.Services.AddScoped<IConsentManager, ConsentManager>();
        
        builder.Services.AddScoped<ITokenValidationProvider, TokenValidationProvider>();
        builder.Services.AddScoped<IJwtTokenValidationProvider, JwtTokenValidationProvider>();
        builder.Services.AddKeyedScoped<ITokenValidator, OpaqueTokenValidator>(TokenKind.Opaque);
        builder.Services.AddKeyedScoped<ITokenValidator, JwtTokenValidator>(TokenKind.Jwt);
        builder.Services.AddKeyedScoped<IJwtTokenValidator, IdTokenValidator>(JwtTokenTypes.IdToken);
        builder.Services.AddKeyedScoped<IJwtTokenValidator, AccessTokenValidator>(JwtTokenTypes.AccessToken);
        builder.Services.AddKeyedScoped<IJwtTokenValidator, GenericJwtTokenValidator>(JwtTokenTypes.Generic);
            
        builder.Services.AddScoped<IAuthorizationCodeManager, AuthorizationCodeManager>();
        builder.Services.AddScoped<IAuthorizationCodeFlowResolver, AuthorizationCodeFlowResolver>();
        builder.Services.AddKeyedScoped<IAuthorizationCodeFlow, OidcAuthorizationCodeFlow>(AuthorizationProtocol.OpenIdConnect);
        builder.Services.AddKeyedScoped<IAuthorizationCodeFlow, OAuthAuthorizationCodeFlow>(AuthorizationProtocol.OAuth);
        
        builder.Services.AddScoped<IRefreshTokenFlowResolver, RefreshTokenFlowResolver>();
        builder.Services.AddKeyedScoped<IRefreshTokenFlow, OidcRefreshTokenFlow>(AuthorizationProtocol.OpenIdConnect);
        builder.Services.AddKeyedScoped<IRefreshTokenFlow, OAuthRefreshTokenFlow>(AuthorizationProtocol.OAuth);
        
        builder.Services.AddScoped<IDeviceCodeFlowResolver, DeviceCodeFlowResolver>();
        builder.Services.AddKeyedScoped<IDeviceCodeFlow, OidcDeviceCodeFlow>(AuthorizationProtocol.OpenIdConnect);
        builder.Services.AddKeyedScoped<IDeviceCodeFlow, OAuthDeviceCodeFlow>(AuthorizationProtocol.OAuth);
            
        builder.Services.AddScoped<ITokenManager, TokenManager>();
        builder.Services.AddScoped<ITokenStrategyResolver, TokenStrategyResolver>();
        builder.Services.AddKeyedScoped<ITokenStrategy, AuthorizationCodeStrategy>(GrantTypes.AuthorizationCode);
        builder.Services.AddKeyedScoped<ITokenStrategy, RefreshTokenStrategy>(GrantTypes.RefreshToken);
        builder.Services.AddKeyedScoped<ITokenStrategy, ClientCredentialsStrategy>(GrantTypes.ClientCredentials);
        builder.Services.AddScoped<ITokenRequestMapper, TokenRequestMapper>();

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