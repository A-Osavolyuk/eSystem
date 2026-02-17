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
            var subjectClaim = principal.Claims.First(x => x.Type == AppClaimTypes.Sub);
            _userState.Subject = subjectClaim.Value;
        }

        if (principal.HasClaim(x => x.Type == AppClaimTypes.PreferredUsername))
        {
            var usernameClaim = principal.Claims.Single(x => x.Type == AppClaimTypes.PreferredUsername);
            _userState.Credentials.Username = usernameClaim.Value;
        }

        if (principal.HasClaim(x => x.Type == AppClaimTypes.Email))
        {
            var emailClaim = principal.Claims.Single(x => x.Type == AppClaimTypes.Email);
            _userState.Credentials.Email = emailClaim.Value;
        }

        if (principal.HasClaim(x => x.Type == AppClaimTypes.PhoneNumber))
        {
            var phoneNumberClaim = principal.Claims.Single(x => x.Type == AppClaimTypes.PhoneNumber);
            _userState.Credentials.PhoneNumber = phoneNumberClaim.Value;
        }
    }
}