using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace eAccount.Application.State;

public static class StateExtensions
{
    public static void AddState(this IHostApplicationBuilder builder)
    {
        builder.Services.AddScoped<UserState>();
    }
}