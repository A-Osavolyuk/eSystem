using eAccount.Security.Authentication;
using eAccount.Security.Authorization;
using eAccount.Security.Cookies;
using eAccount.Security.Credentials;
using eAccount.Security.Cryptography;
using eAccount.Security.Identity;

namespace eAccount.Security;

public static class SecurityExtensions
{
    public static void AddSecurity(this IHostApplicationBuilder builder)
    {
        builder.AddCryptography();
        builder.AddCredentials();
        builder.AddAuthentication();
        builder.AddAuthorization();
        builder.AddCookies();
        builder.AddIdentity();
        
        builder.Services.AddScoped<ISecurityService, SecurityService>();
    }
}