using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ClaimTypes = eShop.Domain.Common.Security.ClaimTypes;

namespace eShop.Infrastructure.Account;

public class JwtAuthenticationHandler(
    IOptionsMonitor<JwtAuthenticationOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    ITokenProvider tokenProvider)
    : AuthenticationHandler<JwtAuthenticationOptions>(options, logger, encoder)
{
    private readonly ITokenProvider tokenProvider = tokenProvider;
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        try
        {
            var token = Request.Cookies["access-token"];

            if (string.IsNullOrEmpty(token))
            {
                return AuthenticateResult.NoResult();
            }

            var rawToken = tokenProvider.ReadToken(token)!;
            var claims = tokenProvider.ReadClaims(rawToken);
            var identity = new ClaimsIdentity(claims, JwtBearerDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, JwtBearerDefaults.AuthenticationScheme);
            
            return AuthenticateResult.Success(ticket);
        }
        catch (Exception e)
        {
            return AuthenticateResult.NoResult();
        }
    }

    protected override Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        Response.Redirect("/account/login");
        return Task.CompletedTask;
    }

    protected override Task HandleForbiddenAsync(AuthenticationProperties properties)
    {
        Response.Redirect("/error?code=403");
        return Task.CompletedTask;
    }
}

