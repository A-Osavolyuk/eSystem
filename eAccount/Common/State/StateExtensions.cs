using eAccount.Common.State.States;

namespace eAccount.Common.State;

public static class StateExtensions
{
    public static void AddState(this IHostApplicationBuilder builder)
    {
        builder.Services.AddScoped<UserState>();
    }
}