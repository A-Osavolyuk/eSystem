using eSecurity.Common.State.States;

namespace eSecurity.Common.State;

public static class StateExtensions
{
    public static void AddState(this IHostApplicationBuilder builder)
    {
        builder.Services.AddScoped<UserState>();
    }
}