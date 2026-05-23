using eSecurity.Idp.Security.Authentication.EndSession.Backchannel;
using eSecurity.Idp.Security.Authentication.EndSession.Frontchannel;

namespace eSecurity.Idp.Security.Authentication.EndSession;

public static class EndSessionExtensions
{
    public static void AddEndSessionFlow(this IServiceCollection services, Action<EndSessionOptions> options)
    {
        services.Configure(options);

        services.AddScoped<IEndSessionManager, EndSessionManager>();
        services.AddScoped<IFrontChannelLogoutHandler, FrontchannelLogoutHandler>();
        services.AddScoped<IBackchannelLogoutHandler, BackchannelLogoutHandler>();
    }
}