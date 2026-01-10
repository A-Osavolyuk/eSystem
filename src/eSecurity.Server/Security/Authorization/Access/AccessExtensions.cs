using eSecurity.Server.Security.Authorization.Access.Codes;
using eSecurity.Server.Security.Authorization.Access.Verification;

namespace eSecurity.Server.Security.Authorization.Access;

public static class AccessExtensions
{
    extension(IServiceCollection services)
    {
        public void AddAccessManagement()
        {
            services.AddScoped<ICodeManager, CodeManager>();
            services.AddScoped<IVerificationManager, VerificationManager>();
        }
    }
}