using eAccount.Application.State;
using eSystem.Core.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace eAccount.Infrastructure.Security.Authentication.JWT;

public class JwtAuthenticationStateProvider(
    IHttpContextAccessor httpContextAccessor,
    UserState userState) : AuthenticationStateProvider
{
    private readonly AuthenticationState anonymous = new(new ClaimsPrincipal());
    private readonly HttpContext httpContext = httpContextAccessor.HttpContext!;

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        if (httpContext.User.Identity?.IsAuthenticated != true) return Task.FromResult(anonymous);

        var principal = httpContext.User;
        var authenticationState = new AuthenticationState(principal);

        var subjectClaim = principal.Claims.Single(x => x.Type == AppClaimTypes.Sub);
        var userId = Guid.Parse(subjectClaim.Value);
        userState.UserId = userId;

        return Task.FromResult(authenticationState);
    }

    public void SignIn(ClaimsPrincipal principal)
    {
        var state = new AuthenticationState(principal);
        NotifyAuthenticationStateChanged(Task.FromResult(state));
    }

    public Task SignOutAsync()
    {
        NotifyAuthenticationStateChanged(Task.FromResult(anonymous));
        return Task.CompletedTask;
    }
}