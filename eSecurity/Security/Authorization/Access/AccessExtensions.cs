using eSecurity.Common.Confirmation;
using eSecurity.Security.Authorization.Access.Codes;
using eSecurity.Security.Authorization.Access.Verification;

namespace eSecurity.Security.Authorization.Access;

public static class AccessExtensions
{
    public static void AddAccessManagement(this IServiceCollection services)
    {
        services.AddScoped<ICodeManager, CodeManager>();
        services.AddScoped<IVerificationManager, VerificationManager>();
    }
}