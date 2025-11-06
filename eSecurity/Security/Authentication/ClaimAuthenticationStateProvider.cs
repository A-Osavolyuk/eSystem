using System.Security.Claims;
using eSecurity.Common.State.States;
using eSystem.Core.Security.Identity.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace eSecurity.Security.Authentication;

public class ClaimAuthenticationStateProvider(
    IHttpContextAccessor httpContextAccessor,
    UserState userState) : AuthenticationStateProvider
{
    private readonly AuthenticationState anonymous = new(new ClaimsPrincipal());
    private readonly HttpContext httpContext = httpContextAccessor.HttpContext!;
    private readonly UserState userState = userState;

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        if (httpContext.User.Identity?.IsAuthenticated != true) return Task.FromResult(anonymous);
        
        var principal = httpContext.User;
        var claims = httpContext.User.Claims.ToList();
        var userId = Guid.Parse(claims.Single(x => x.Type == AppClaimTypes.Sub).Value);
        
        userState.UserId = userId;
        userState.Credentials = new UserCredentials();

        if (principal.HasClaim(x => x.Type == AppClaimTypes.PreferredUsername))
        {
            var username = claims.Single(x => x.Type == AppClaimTypes.PreferredUsername).Value;
            userState.Credentials.Username = username;
        }

        if (principal.HasClaim(x => x.Type == AppClaimTypes.Email))
        {
            var email = claims.Single(x => x.Type == AppClaimTypes.Email).Value;
            userState.Credentials.Email = email;
        }

        if (principal.HasClaim(x => x.Type == AppClaimTypes.PhoneNumber))
        {
            var phoneNumber = claims.Single(x => x.Type == AppClaimTypes.PhoneNumber).Value;
            userState.Credentials.PhoneNumber = phoneNumber;
        }
        
        return Task.FromResult(new AuthenticationState(principal));
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