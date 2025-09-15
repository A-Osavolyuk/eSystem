using eShop.Blazor.Application.State;
using eShop.Blazor.Domain.Interfaces;
using eShop.Domain.Common.Security.Constants;
using eShop.Domain.DTOs;
using eShop.Domain.Requests.API.Auth;
using eShop.Domain.Responses.API.Auth;

namespace eShop.Blazor.Infrastructure.Security;

public class JwtAuthenticationStateProvider(
    ITokenProvider tokenProvider,
    IStorage storage,
    ISecurityService securityService,
    IUserService userService,
    TokenHandler tokenHandler,
    UserState userState) : AuthenticationStateProvider
{
    private readonly AuthenticationState anonymous = new(new ClaimsPrincipal());
    private readonly ITokenProvider tokenProvider = tokenProvider;
    private readonly IStorage storage = storage;
    private readonly ISecurityService securityService = securityService;
    private readonly IUserService userService = userService;
    private readonly TokenHandler tokenHandler = tokenHandler;
    private readonly UserState userState = userState;

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var token = await tokenProvider.GetAsync();
            if (string.IsNullOrEmpty(token)) return await UnauthorizeAsync();

            var valid = tokenHandler.Validate(token);
            if (!valid) return await RefreshTokenAsync(token);

            var rawToken = tokenHandler.ReadToken(token);
            if (rawToken is null || !rawToken.Claims.Any()) return await UnauthorizeAsync();

            var claims = rawToken.Claims.ToList();
            if (!userState.IsAuthenticated) await LoadAsync(claims);

            var claimsIdentity = new ClaimsIdentity(claims, JwtBearerDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            var authenticationState = new AuthenticationState(claimsPrincipal);

            return await Task.FromResult(authenticationState);
        }
        catch (Exception)
        {
            return await UnauthorizeAsync();
        }
    }

    public async Task SignInAsync(string accessToken, string refreshToken)
    {
        if (!string.IsNullOrEmpty(accessToken) && !string.IsNullOrEmpty(refreshToken))
        {
            var rawToken = tokenHandler.ReadToken(accessToken)!;
            var claims = rawToken.Claims.ToList();

            var userId = Guid.Parse(claims.First(x => x.Type == AppClaimTypes.Subject).Value);
            var jti = Guid.Parse(claims.First(x => x.Type == AppClaimTypes.Jti).Value);
            var exp = long.Parse(claims.First(x => x.Type == AppClaimTypes.Exp).Value);

            await storage.SetAsync("userId", userId);
            await storage.SetAsync("jti", jti);
            await storage.SetAsync("exp", exp);

            userState.UserId = userId;

            var identity = new ClaimsIdentity(claims, JwtBearerDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(identity);
            var authenticationState = new AuthenticationState(claimsPrincipal);

            NotifyAuthenticationStateChanged(Task.FromResult(authenticationState));
        }
        else await UnauthorizeAsync();
    }

    public async Task<AuthenticationState> SignOutAsync()
    {
        await storage.ClearAsync();
        userState.Clear();
        return await UnauthorizeAsync();
    }

    private async Task<AuthenticationState> UnauthorizeAsync()
    {
        NotifyAuthenticationStateChanged(Task.FromResult(anonymous));
        return await Task.FromResult(anonymous);
    }

    private async Task<AuthenticationState> RefreshTokenAsync(string token)
    {
        var request = new RefreshTokenRequest()
        {
            Token = token,
            UserId = userState.UserId
        };

        var result = await securityService.RefreshTokenAsync(request);

        if (!result.Success) return await SignOutAsync();

        var response = result.Get<RefreshTokenResponse>()!;
        var rawToken = tokenHandler.ReadToken(response.RefreshToken)!;
        var claims = rawToken.Claims.ToList();
        var identity = new ClaimsIdentity(claims, JwtBearerDefaults.AuthenticationScheme);
        var claimsPrincipal = new ClaimsPrincipal(identity);
        var authenticationState = new AuthenticationState(claimsPrincipal);

        return authenticationState;
    }

    private async Task LoadAsync(List<Claim> claims)
    {
        var userId = Guid.Parse(claims.Single(x => x.Type == AppClaimTypes.Subject).Value);
        var result = await userService.GetUserStateAsync(userId);

        if (result.Success)
        {
            var state = result.Get<UserStateDto>()!;
            userState.Map(state);
        }
    }
}