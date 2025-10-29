using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace eSystem.Core.Security.Authentication.JWT;

public static class JwtExtensions
{
    public static void AddJwtAuthentication(this IServiceCollection services)
    {
        var configuration = services.BuildServiceProvider().GetService<IConfiguration>()!;
        
        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                var jwtOption = configuration.GetSection("JWT").Get<JwtOptions>()!;
                var keyBytes = Encoding.UTF8.GetBytes(jwtOption.Secret);
                var securityKey = new SymmetricSecurityKey(keyBytes);
            
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    RequireExpirationTime = true,
                    ValidAudiences = jwtOption.Audiences,
                    ValidIssuer = jwtOption.Issuer,
                    IssuerSigningKey = securityKey
                };
            });
    }
}