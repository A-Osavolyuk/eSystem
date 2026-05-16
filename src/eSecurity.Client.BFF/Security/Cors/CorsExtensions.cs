namespace eSecurity.Client.BFF.Security.Cors;

public static class CorsExtensions
{
    public static void AddCors(this IHostApplicationBuilder builder)
    {
        builder.Services.AddCors(options =>
        {
            options.AddPolicy(CorsPolicies.SpaOnly, policy =>
            {
                policy.WithOrigins("https://localhost:6521");
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