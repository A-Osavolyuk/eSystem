using eShop.Domain.Requests.API.Auth;
using eShop.Domain.Responses.API.Auth;
using ClaimTypes = eShop.Domain.Common.Security.ClaimTypes;
using UserModel = eShop.Domain.Models.UserModel;

namespace eShop.Infrastructure.Account;

public class JwtAuthenticationStateProvider(
    ITokenProvider tokenProvider,
    ISecurityService securityService,
    IStorage localStorage,
    IUserStorage userStorage,
    JwtAuthenticationHandler jwtAuthenticationHandler) : AuthenticationStateProvider
{
    private readonly AuthenticationState anonymous = new(new ClaimsPrincipal());
    private readonly ITokenProvider tokenProvider = tokenProvider;
    private readonly ISecurityService securityService = securityService;
    private readonly IStorage localStorage = localStorage;
    private readonly IUserStorage userStorage = userStorage;
    private readonly JwtAuthenticationHandler jwtAuthenticationHandler = jwtAuthenticationHandler;

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var token = await tokenProvider.GetTokenAsync();
            if (string.IsNullOrEmpty(token))
            {
                return await Task.FromResult(anonymous);
            }

            var rowToken = tokenProvider.ReadToken(token);

            if (rowToken is null || !rowToken.Claims.Any())
            {
                return await Task.FromResult(anonymous);
            }

            var valid = tokenProvider.IsValid(rowToken);

            if (!valid)
            {
                await LogOutAsync();
                return await Task.FromResult(anonymous);
            }

            var claims = tokenProvider.ReadClaims(rowToken);

            if (claims.Count == 0)
            {
                return await Task.FromResult(anonymous);
            }

            return await Task.FromResult(
                new AuthenticationState(new ClaimsPrincipal(
                    new ClaimsIdentity(claims, JwtBearerDefaults.AuthenticationScheme))));
        }
        catch (Exception)
        {
            return await Task.FromResult(anonymous);
        }
    }

    public async Task UpdateAuthenticationStateAsync(string token)
    {
        var claimsPrincipal = new ClaimsPrincipal();

        if (!string.IsNullOrEmpty(token))
        {
            await tokenProvider.SetTokenAsync(token);

            var rawToken = tokenProvider.ReadToken(token)!;
            var claims = tokenProvider.ReadClaims(rawToken);
            claimsPrincipal = new(new ClaimsIdentity(claims, JwtBearerDefaults.AuthenticationScheme));
        }

        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
    }

    public async Task LoginAsync(string accessToken, string refreshToken)
    {
        var claimsPrincipal = new ClaimsPrincipal();

        if (!string.IsNullOrEmpty(accessToken) && !string.IsNullOrEmpty(refreshToken))
        {
            await tokenProvider.SetTokenAsync(refreshToken);

            var rawToken = tokenProvider.ReadToken(accessToken)!;
            var claims = tokenProvider.ReadClaims(rawToken);
            claimsPrincipal = new(new ClaimsIdentity(claims, JwtBearerDefaults.AuthenticationScheme));
        }
        
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
    }

    public async Task LogOutAsync()
    {
        await tokenProvider.ClearAsync();
        await localStorage.ClearAsync();
        await userStorage.ClearAsync();
        await UpdateAuthenticationStateAsync(string.Empty);
    }
}