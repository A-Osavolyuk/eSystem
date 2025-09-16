using eShop.Blazor.Application.State;
using eShop.Blazor.Domain.Interfaces;
using eShop.Domain.Common.Security.Constants;
using eShop.Domain.DTOs;
using eShop.Domain.Responses.API.Auth;

namespace eShop.Blazor.Infrastructure.Security;

public class JwtAuthenticationStateProvider(
    ITokenProvider tokenProvider,
    IStorage storage,
    IUserService userService,
    ISecurityService securityService,
    TokenHandler tokenHandler,
    UserState userState) : AuthenticationStateProvider
{
    private readonly AuthenticationState anonymous = new(new ClaimsPrincipal());
    private readonly ITokenProvider tokenProvider = tokenProvider;
    private readonly IStorage storage = storage;
    private readonly IUserService userService = userService;
    private readonly ISecurityService securityService = securityService;
    private readonly TokenHandler tokenHandler = tokenHandler;
    private readonly UserState userState = userState;

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var token = tokenProvider.Get();
            if (string.IsNullOrEmpty(token))
            {
                var result = await securityService.AuthenticateAsync();
                if (result.Success)
                {
                    var response = result.Get<AuthenticateResponse>()!;
                    
                    token = response.AccessToken;
                    tokenProvider.Set(token);
                }
                else return await UnauthorizeAsync();
            }

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

    public async Task SignInAsync(string accessToken)
    {
        if (!string.IsNullOrEmpty(accessToken))
        {
            tokenProvider.Set(accessToken);

            var rawToken = tokenHandler.ReadToken(accessToken)!;
            var claims = rawToken.Claims.ToList();

            var userId = Guid.Parse(claims.First(x => x.Type == AppClaimTypes.Subject).Value);
            userState.UserId = userId;

            var identity = new ClaimsIdentity(claims, JwtBearerDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(identity);
            var authenticationState = new AuthenticationState(claimsPrincipal);

            NotifyAuthenticationStateChanged(Task.FromResult(authenticationState));
        }
        else await UnauthorizeAsync();
    }

    public async Task SignOutAsync()
    {
        await storage.ClearAsync();

        userState.Clear();
        tokenProvider.Clear();

        await UnauthorizeAsync();
    }

    private async Task<AuthenticationState> UnauthorizeAsync()
    {
        NotifyAuthenticationStateChanged(Task.FromResult(anonymous));

        return await Task.FromResult(anonymous);
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