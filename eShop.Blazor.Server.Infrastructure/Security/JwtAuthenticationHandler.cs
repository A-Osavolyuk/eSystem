using System.Text.Encodings.Web;
using eShop.Blazor.Server.Domain.Interfaces;
using eShop.Domain.Requests.API.Auth;
using eShop.Domain.Responses.API.Auth;
using Microsoft.AspNetCore.Authentication;
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

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        try
        {
            var accessToken = tokenProvider.AccessToken;
            if (string.IsNullOrEmpty(accessToken))
            {
                var refreshToken = tokenProvider.RefreshToken;
                if (string.IsNullOrEmpty(refreshToken)) return AuthenticateResult.NoResult();
            
                var request = new AuthenticateRequest() { RefreshToken = refreshToken };
                var result = await securityService.AuthenticateAsync(request);
                if (!result.Success) return AuthenticateResult.NoResult();

                var response = result.Get<AuthenticateResponse>()!;
                accessToken = response.AccessToken;
                tokenProvider.AccessToken = response.AccessToken;
            }

            var rawToken = tokenHandler.ReadToken(accessToken)!;
            var claims = rawToken.Claims.ToList();
            var identity = new ClaimsIdentity(claims, JwtBearerDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, JwtBearerDefaults.AuthenticationScheme);
            
            return AuthenticateResult.Success(ticket);
        }
        catch (Exception)
        {
            return AuthenticateResult.NoResult();
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

