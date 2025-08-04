using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace eShop.Infrastructure.Security;

public class JwtAuthenticationHandler(
    IOptionsMonitor<JwtAuthenticationOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    TokenHandler tokenHandler,
    JwtTokenStorage jwtTokenStorage)
    : AuthenticationHandler<JwtAuthenticationOptions>(options, logger, encoder)
{
    private readonly TokenHandler tokenHandler = tokenHandler;
    private readonly JwtTokenStorage jwtTokenStorage = jwtTokenStorage;

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        try
        {
            var token = Request.Cookies["access-token"];

            if (string.IsNullOrEmpty(token))
            {
                return Task.FromResult(AuthenticateResult.NoResult());
            }
            
            jwtTokenStorage.Token = token;

            var rawToken = tokenHandler.ReadToken(token)!;
            var claims = rawToken.Claims.ToList();
            var identity = new ClaimsIdentity(claims, JwtBearerDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, JwtBearerDefaults.AuthenticationScheme);
            
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

