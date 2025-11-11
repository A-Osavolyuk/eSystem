using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;

namespace eSecurity.Client.Security.Authentication.Handlers.Jwt;

public class JwtAuthenticationHandler(
    IOptionsMonitor<JwtAuthenticationOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder) : AuthenticationHandler<JwtAuthenticationOptions>(options, logger, encoder)
{
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        //TODO: Implement ID token validation

        var claims = new List<Claim>();
        var claimIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var claimPrincipal = new ClaimsPrincipal(claimIdentity);
        var ticket = new AuthenticationTicket(claimPrincipal, CookieAuthenticationDefaults.AuthenticationScheme);
        return AuthenticateResult.Success(ticket);
    }
}