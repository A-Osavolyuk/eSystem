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
        if (_httpContext.User.Identity?.IsAuthenticated != true)
            return Task.FromResult(_anonymous);
        
        InitializeState(_httpContext.User);
        
        return Task.FromResult(new AuthenticationState(_httpContext.User));
    }

    public void SignIn(ClaimsPrincipal principal)
    {
        InitializeState(principal);
        
        var state = new AuthenticationState(principal);
        NotifyAuthenticationStateChanged(Task.FromResult(state));
    }

    public Task SignOutAsync()
    {
        _userState.Clear();
        NotifyAuthenticationStateChanged(Task.FromResult(_anonymous));
        return Task.CompletedTask;
    }
    
    public Task NotifyAsync()
    {
        NotifyAuthenticationStateChanged(Task.FromResult(_anonymous));
        return Task.CompletedTask;
    }

    private void InitializeState(ClaimsPrincipal principal)
    {
        _userState.Credentials = new UserCredentials();
        
        if (principal.HasClaim(x => x.Type == AppClaimTypes.Sub))
        {
            var userId = principal.Claims.Single(x => x.Type == AppClaimTypes.Sub).Value;
            _userState.UserId = Guid.Parse(userId);
        }

        if (principal.HasClaim(x => x.Type == AppClaimTypes.PreferredUsername))
        {
            var username = principal.Claims.Single(x => x.Type == AppClaimTypes.PreferredUsername).Value;
            _userState.Credentials.Username = username;
        }

        if (principal.HasClaim(x => x.Type == AppClaimTypes.Email))
        {
            var email = principal.Claims.Single(x => x.Type == AppClaimTypes.Email).Value;
            _userState.Credentials.Email = email;
        }

        if (principal.HasClaim(x => x.Type == AppClaimTypes.PhoneNumber))
        {
            var phoneNumber = principal.Claims.Single(x => x.Type == AppClaimTypes.PhoneNumber).Value;
            _userState.Credentials.PhoneNumber = phoneNumber;
        }
    }
}