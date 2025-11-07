using eSecurity.Common.Confirmation;

namespace eSecurity.Security.Authorization.Access;

public static class AccessExtensions
{
    public static void AddAccessManagement(this IServiceCollection services)
    {
        services.AddScoped<ICodeManager, CodeManager>();
        services.AddScoped<IVerificationManager, VerificationManager>();
    }
}