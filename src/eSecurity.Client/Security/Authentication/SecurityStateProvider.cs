using System.Security.Claims;
using eSecurity.Client.Common.State;
using eSecurity.Client.Security.Authentication.OpenIdConnect;
using eSystem.Core.Security.Authentication.OpenIdConnect.User;
using eSystem.Core.Security.Identity.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace eSecurity.Client.Security.Authentication;

public sealed class SecurityStateProvider(
    IConnectService connectService,
    UserState userState) : AuthenticationStateProvider
{
    private readonly IConnectService _connectService = connectService;
    private readonly UserState _userState = userState;
    private readonly AuthenticationState _anonymous = new (new ClaimsPrincipal());

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var result = await _connectService.UserInfoAsync();
        if (result.Succeeded && result.TryGetValue<UserInfoResponse>(out var response) && response is not null)
        {
            var claims = new List<Claim>();

            if (!string.IsNullOrEmpty(response.Email))
            {
                _userState.Email = response.Email;
                claims.Add(new Claim(AppClaimTypes.Email, response.Email));
            }

            if (!string.IsNullOrEmpty(response.PreferredUsername))
            {
                _userState.Username = response.PreferredUsername;
                claims.Add(new Claim(AppClaimTypes.PreferredUsername, response.PreferredUsername));
            }

            if (claims.Count == 0)
                return _anonymous;

            var claimsIdentity = new ClaimsIdentity(claims, "Cookie");
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            return new AuthenticationState(claimsPrincipal);
        }

        return _anonymous;
    }
}