using eSecurity.Idp.Security.Authorization.Authorize;
using eSecurity.Idp.Security.Authorization.Authorize.Par;
using eSecurity.Idp.Security.Authorization.Codes;
using eSecurity.Idp.Security.Authorization.Consents;
using eSecurity.Idp.Security.Authorization.Constants;
using eSecurity.Idp.Security.Authorization.Devices.Extensions;
using eSecurity.Idp.Security.Authorization.LinkedAccount;
using eSecurity.Idp.Security.Authorization.Prompt;
using eSecurity.Idp.Security.Authorization.Roles;
using eSecurity.Idp.Security.Authorization.Token;
using eSecurity.Idp.Security.Authorization.Token.DeviceCode;
using eSecurity.Idp.Security.Authorization.Token.Validation;
using eSecurity.Idp.Security.Authorization.Verification.Extensions;
using eSystem.Core.Security.Authorization.OAuth;
using eSystem.Core.Server.Security.Authentication.Schemes;
using eSystem.Core.Server.Security.Authorization.OAuth.Token.Validation;

namespace eSecurity.Idp.Security.Authorization;

public static class AuthorizationExtensions
{
    public static void AddAuthorization(this IHostApplicationBuilder builder)
    {
        var configuration = builder.Configuration;
        var clientUri = configuration.GetValue<string>("Client:Uri");
        
        builder.Services.AddAuthorizationFlow(options =>
        {
            options.FallbackUrl = $"{clientUri}/connect/fallback";
        });
        
        builder.Services.AddPromptsHandling();
        builder.Services.AddTokenFlow();
        builder.Services.AddRoleManagement();
        builder.Services.AddDeviceManagement();
        builder.Services.AddDeviceAuthorization(options =>
        {
            options.DeviceCodeLenght = 32;
            options.UserCodeLenght = 8;
            options.Interval = 5;
            options.Timestamp = TimeSpan.FromSeconds(3600);
            options.VerificationUri = $"{clientUri}/device/activate";
        });
        
        builder.Services.AddVerification(options =>
        {
            options.Timestamp = TimeSpan.FromMinutes(10);
        });

        builder.Services.AddScoped<ICodeQueryService, CodeQueryService>();
        builder.Services.AddScoped<ICodeCommandService, CodeCommandService>();
        
        builder.Services.AddScoped<IConsentManager, ConsentManager>();
        builder.Services.AddScoped<ILinkedAccountManager, LinkedAccountManager>();
        builder.Services.AddScoped<ITokenValidationProvider, TokenValidationProvider>();
        builder.Services.AddScoped<IJwtTokenValidationProvider, JwtTokenValidationProvider>();
        builder.Services.AddKeyedScoped<ITokenValidator, OpaqueTokenValidator>(TokenKind.Opaque);
        builder.Services.AddKeyedScoped<ITokenValidator, JwtTokenValidator>(TokenKind.Jwt);
        builder.Services.AddKeyedScoped<IJwtTokenValidator, IdTokenValidator>(JwtTokenType.IdToken);
        builder.Services.AddKeyedScoped<IJwtTokenValidator, AccessTokenValidator>(JwtTokenType.AccessToken);
        builder.Services.AddKeyedScoped<IJwtTokenValidator, GenericJwtTokenValidator>(JwtTokenType.Generic);
        builder.Services.AddTransient<IParManager, ParManager>();

        builder.Services.AddAuthorizationBuilder()
            .AddPolicy(AuthorizationPolicies.BasicAuthorization, policy =>
            {
                policy.AddAuthenticationSchemes(
                    AuthenticationSchemes.ClientSecretBasic,
                    AuthenticationSchemes.ClientSecretPost);
                policy.RequireAuthenticatedUser();
            })
            .AddPolicy(AuthorizationPolicies.TokenAuthorization, policy =>
            {
                policy.AddAuthenticationSchemes(
                    AuthenticationSchemes.ClientSecretBasic,
                    AuthenticationSchemes.ClientSecretPost);
                policy.RequireAuthenticatedUser();
            });
    }
}