using eSecurity.Server.Security.Authorization.Access;
using eSecurity.Server.Security.Authorization.Consents;
using eSecurity.Server.Security.Authorization.Devices;
using eSecurity.Server.Security.Authorization.OAuth;
using eSecurity.Server.Security.Authorization.Roles;
using eSecurity.Server.Security.Authorization.Scopes;

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
        builder.Services.AddScoped<IScopeManager, ScopeManager>();
    }
}