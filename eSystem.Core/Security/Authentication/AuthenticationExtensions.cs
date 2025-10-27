using System.Text;
using eSystem.Core.Common.Configuration;
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
                const string audiencesPath = "JWT:Audiences";
                const string issuerPath = "JWT:Issuer";
                const string keyPath = "JWT:Secret";
                
                var audiences = builder.Configuration.Get<List<string>>(audiencesPath);
                var keyBytes = Encoding.UTF8.GetBytes(builder.Configuration[keyPath]!);
                var securityKey = new SymmetricSecurityKey(keyBytes);
            
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    RequireExpirationTime = true,
                    ValidAudiences = audiences,
                    ValidIssuer = builder.Configuration[issuerPath],
                    IssuerSigningKey = securityKey
                };
            });
    }
}