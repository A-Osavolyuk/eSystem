using System.Text;
using eSystem.Core.Common.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace eSystem.Core.Security.Authentication.JWT;

public static class JwtExtensions
{
    public static void AddJwt(this IServiceCollection services)
    {
        var configuration = services.BuildServiceProvider().GetService<IConfiguration>()!;
        
        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                const string audiencesPath = "JWT:Audiences";
                const string issuerPath = "JWT:Issuer";
                const string keyPath = "JWT:Secret";
                
                var audiences = configuration.Get<List<string>>(audiencesPath);
                var keyBytes = Encoding.UTF8.GetBytes(configuration[keyPath]!);
                var securityKey = new SymmetricSecurityKey(keyBytes);
            
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    RequireExpirationTime = true,
                    ValidAudiences = audiences,
                    ValidIssuer = configuration[issuerPath],
                    IssuerSigningKey = securityKey
                };
            });
    }
}