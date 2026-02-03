using eSecurity.Server.Security.Authorization.Access;
using eSecurity.Server.Security.Authorization.Consents;
using eSecurity.Server.Security.Authorization.Constants;
using eSecurity.Server.Security.Authorization.Devices;
using eSecurity.Server.Security.Authorization.OAuth;
using eSecurity.Server.Security.Authorization.Permissions;
using eSecurity.Server.Security.Authorization.Protocol;
using eSecurity.Server.Security.Authorization.Roles;
using eSecurity.Server.Security.Authorization.Scopes;
using eSecurity.Server.Security.Authorization.Token;
using eSecurity.Server.Security.Authorization.Token.AuthorizationCode;
using eSecurity.Server.Security.Authorization.Token.RefreshToken;
using eSecurity.Server.Security.Authorization.Token.Strategies;
using eSecurity.Server.Security.Authorization.Token.Validation;
using eSystem.Core.Security.Authentication.OpenIdConnect.Constants;
using eSystem.Core.Security.Authentication.OpenIdConnect.Token.Validation;
using eSystem.Core.Security.Authentication.Schemes;

namespace eSecurity.Server.Security.Authorization;

public static class AuthorizationExtensions
{
    public static void AddAuthorization(this IHostApplicationBuilder builder)
    {
        builder.Services.AddRoleManagement();
        builder.Services.AddAccessManagement();
        builder.Services.AddDeviceManagement();
        builder.Services.AddOAuthAuthorization();

        builder.Services.AddScoped<IConsentManager, ConsentManager>();
        builder.Services.AddScoped<IPermissionManager, PermissionManager>();
        builder.Services.AddScoped<IScopeManager, ScopeManager>();
        
        builder.Services.AddScoped<ITokenValidationProvider, TokenValidationProvider>();
        builder.Services.AddScoped<IJwtTokenValidationProvider, JwtTokenValidationProvider>();
        builder.Services.AddKeyedScoped<ITokenValidator, OpaqueTokenValidator>(TokenTypes.Opaque);
        builder.Services.AddKeyedScoped<ITokenValidator, JwtTokenValidator>(TokenTypes.Jwt);
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
            
        builder.Services.AddScoped<ITokenManager, TokenManager>();
        builder.Services.AddScoped<ITokenStrategyResolver, TokenStrategyResolver>();
        builder.Services.AddKeyedScoped<ITokenStrategy, AuthorizationCodeStrategy>(GrantTypes.AuthorizationCode);
        builder.Services.AddKeyedScoped<ITokenStrategy, RefreshTokenStrategy>(GrantTypes.RefreshToken);
        builder.Services.AddKeyedScoped<ITokenStrategy, ClientCredentialsStrategy>(GrantTypes.ClientCredentials);
        builder.Services.AddScoped<ITokenContextFactory, TokenContextFactory>();

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