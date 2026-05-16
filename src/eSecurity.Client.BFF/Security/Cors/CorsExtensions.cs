using eSystem.Core.Configuration;
using eSystem.Core.Server.Bff;

namespace eSecurity.Client.BFF.Security.Cors;

public static class CorsExtensions
{
    public static void AddCors(this IHostApplicationBuilder builder)
    {
        builder.Services.AddCors(options =>
        {
            var bffOptions = builder.Configuration.Get<BffOptions>("Bff");
            options.AddPolicy(CorsPolicies.SpaOnly, policy =>
            {
                policy.WithOrigins(bffOptions.FrontendUri);
                policy.AllowAnyHeader();
                policy.AllowAnyMethod();
                policy.AllowCredentials();
            });
                
            options.AddPolicy(CorsPolicies.ExternalOnly, policy =>
            {
                policy.AllowAnyOrigin();
                policy.AllowAnyHeader();
                policy.AllowAnyMethod();
            });
        });
    }
}