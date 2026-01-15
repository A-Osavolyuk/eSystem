using eSecurity.Client.Common.State.States;

namespace eSecurity.Client.Common.State;

public static class StateExtensions
{
    public static void AddState(this IServiceCollection services)
    {
        services.AddScoped<UserState>();
        services.AddScoped<SessionState>();
    }
}