using eSecurity.Server.Security.Authorization.Access;
using eSecurity.Server.Security.Authorization.Consents;
using eSecurity.Server.Security.Authorization.Constants;
using eSecurity.Server.Security.Authorization.Devices;
using eSecurity.Server.Security.Authorization.OAuth;
using eSecurity.Server.Security.Authorization.Permissions;
using eSecurity.Server.Security.Authorization.Roles;
using eSecurity.Server.Security.Authorization.Scopes;
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