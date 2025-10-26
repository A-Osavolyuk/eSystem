using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace eSystem.Core.Security.Authentication;

public static class AuthenticationExtensions
{
    public static void AddJwtAuthentication(this IHostApplicationBuilder builder)
    {
        builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                const string audiencePath = "JWT:Audience";
                const string issuerPath = "JWT:Issuer";
                const string keyPath = "JWT:Secret";
            
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    RequireExpirationTime = true,
                    ValidAudience = builder.Configuration[audiencePath],
                    ValidIssuer = builder.Configuration[issuerPath],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration[keyPath]!))
                };
            });
    }
}