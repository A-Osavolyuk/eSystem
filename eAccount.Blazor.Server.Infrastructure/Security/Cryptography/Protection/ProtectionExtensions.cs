using eSystem.Core.Security.Cryptography.Protection;

namespace eAccount.Blazor.Server.Infrastructure.Security.Cryptography.Protection;

public static class ProtectionExtensions
{
    public static void AddProtection(this IServiceCollection services)
    {
        services.AddDataProtection();
        services.AddScoped<IProtector, SessionProtector>();
    }
}