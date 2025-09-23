using System.Text.Encodings.Web;
using eShop.Blazor.Server.Domain.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace eShop.Blazor.Server.Infrastructure.Security;

public class JwtAuthenticationHandler(
    IOptionsMonitor<JwtAuthenticationOptions> options,
    ILoggerFactory logger,
    ISecurityService securityService,
    TokenProvider tokenProvider,
    UrlEncoder encoder,
    TokenHandler tokenHandler)
    : AuthenticationHandler<JwtAuthenticationOptions>(options, logger, encoder)
{
    private readonly ISecurityService securityService = securityService;
    private readonly TokenProvider tokenProvider = tokenProvider;
    private readonly TokenHandler tokenHandler = tokenHandler;

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        try
        {
            var accessToken = tokenProvider.AccessToken;
            if (string.IsNullOrEmpty(accessToken)) return Task.FromResult(AuthenticateResult.NoResult());

            var rawToken = tokenHandler.ReadToken(accessToken)!;
            var claims = rawToken.Claims.ToList();
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, CookieAuthenticationDefaults.AuthenticationScheme);
            
            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
        catch (Exception)
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }
    }

    protected override Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        Response.Redirect("/error?code=401");
        return Task.CompletedTask;
    }

    protected override Task HandleForbiddenAsync(AuthenticationProperties properties)
    {
        Response.Redirect("/error?code=403");
        return Task.CompletedTask;
    }
}

