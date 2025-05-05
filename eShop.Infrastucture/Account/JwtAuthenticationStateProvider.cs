using eShop.Domain.Requests.API.Auth;
using eShop.Domain.Responses.API.Auth;
using ClaimTypes = eShop.Domain.Common.Security.ClaimTypes;
using UserModel = eShop.Domain.Models.UserModel;

namespace eShop.Infrastructure.Account;

public class JwtAuthenticationStateProvider(
    ITokenProvider tokenProvider,
    IStorage localStorage,
    IUserStorage userStorage) : AuthenticationStateProvider
{
    private readonly AuthenticationState anonymous = new(new ClaimsPrincipal());
    private readonly ITokenProvider tokenProvider = tokenProvider;
    private readonly IStorage localStorage = localStorage;
    private readonly IUserStorage userStorage = userStorage;

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var token = await tokenProvider.GetTokenAsync();
            if (string.IsNullOrEmpty(token))
            {
                return await UnauthorizeAsync();
            }

            var rowToken = tokenProvider.ReadToken(token);

            if (rowToken is null || !rowToken.Claims.Any())
            {
                return await UnauthorizeAsync();
            }

            var valid = tokenProvider.IsValid(rowToken);

            if (!valid)
            {
                return await LogOutAsync();
            }

            var claims = tokenProvider.ReadClaims(rowToken);

            if (claims.Count == 0)
            {
                return await UnauthorizeAsync();
            }

            return await Task.FromResult(
                new AuthenticationState(new ClaimsPrincipal(
                    new ClaimsIdentity(claims, JwtBearerDefaults.AuthenticationScheme))));
        }
        catch (Exception)
        {
            return await UnauthorizeAsync();
        }
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

    public async Task<AuthenticationState> LogOutAsync()
    {
        await tokenProvider.RemoveAsync();
        await localStorage.ClearAsync();
        await userStorage.ClearAsync();
        return await UnauthorizeAsync();
    }

    private async Task<AuthenticationState> UnauthorizeAsync()
    {
        try
        {
            NotifyAuthenticationStateChanged(Task.FromResult(anonymous));
            return await Task.FromResult(anonymous);
        }
        catch (Exception e)
        {
            return await Task.FromResult(anonymous);
        }
    }
}