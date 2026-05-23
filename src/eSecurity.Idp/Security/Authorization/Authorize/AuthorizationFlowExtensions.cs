using eSecurity.Idp.Security.Authorization.Authorize.Manual;
using eSecurity.Idp.Security.Authorization.Authorize.Par;

namespace eSecurity.Idp.Security.Authorization.Authorize;

public static class AuthorizationFlowExtensions
{
    public static void AddAuthorizationFlow(this IServiceCollection services, Action<AuthorizationOptions> options)
    {
        services.Configure(options);

        services.AddTransient<RedirectManager>();
        services.AddScoped<IAuthorizationFlowHandlerProvider, AuthorizationFlowHandlerProvider>();
        services.AddKeyedScoped<IAuthorizationFlowHandler, ManualAuthorizationFlowHandler>(AuthorizationFlow.Manual);
        services.AddKeyedScoped<IAuthorizationFlowHandler, ParAuthorizationFlowHandler>(AuthorizationFlow.PushedAuthorizationRequest);
    }
}