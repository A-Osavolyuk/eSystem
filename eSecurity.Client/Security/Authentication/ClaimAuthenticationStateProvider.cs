using System.Security.Claims;
using eSecurity.Client.Common.State.States;
using eSystem.Core.Security.Identity.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace eSecurity.Client.Security.Authentication;

public class ClaimAuthenticationStateProvider(
    IHttpContextAccessor httpContextAccessor,
    UserState userState) : AuthenticationStateProvider
{
    private readonly AuthenticationState _anonymous = new(new ClaimsPrincipal());
    private readonly HttpContext _httpContext = httpContextAccessor.HttpContext!;
    private readonly UserState _userState = userState;

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        if (_httpContext.User.Identity?.IsAuthenticated != true) return Task.FromResult(_anonymous);
        
        var principal = _httpContext.User;
        var claims = _httpContext.User.Claims.ToList();
        var userId = Guid.Parse(claims.Single(x => x.Type == ClaimTypes.NameIdentifier).Value);
        
        _userState.UserId = userId;
        _userState.Credentials = new UserCredentials();

        if (principal.HasClaim(x => x.Type == AppClaimTypes.PreferredUsername))
        {
            var username = claims.Single(x => x.Type == AppClaimTypes.PreferredUsername).Value;
            _userState.Credentials.Username = username;
        }

        if (principal.HasClaim(x => x.Type == AppClaimTypes.Email))
        {
            var email = claims.Single(x => x.Type == AppClaimTypes.Email).Value;
            _userState.Credentials.Email = email;
        }

        if (principal.HasClaim(x => x.Type == AppClaimTypes.PhoneNumber))
        {
            var phoneNumber = claims.Single(x => x.Type == AppClaimTypes.PhoneNumber).Value;
            _userState.Credentials.PhoneNumber = phoneNumber;
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
        NotifyAuthenticationStateChanged(Task.FromResult(_anonymous));
        return Task.CompletedTask;
    }
}