using eShop.Domain.Common.Security;
using eShop.Domain.Stores;

namespace eShop.Infrastructure.Security;

public class JwtAuthenticationStateProvider(
    ITokenProvider tokenProvider,
    IStorage localStorage,
    IUserStorage userStorage,
    TokenHandler tokenHandler) : AuthenticationStateProvider
{
    private readonly AuthenticationState anonymous = new(new ClaimsPrincipal());
    private readonly ITokenProvider tokenProvider = tokenProvider;
    private readonly IStorage localStorage = localStorage;
    private readonly IUserStorage userStorage = userStorage;
    private readonly TokenHandler tokenHandler = tokenHandler;

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var token = await tokenProvider.GetTokenAsync();
            
            if (string.IsNullOrEmpty(token))
            {
                return await UnauthorizeAsync();
            }
            
            var valid = tokenHandler.Validate(token);

            if (!valid)
            {
                return await LogOutAsync();
            }

            var rowToken = tokenHandler.ReadToken(token);

            if (rowToken is null || !rowToken.Claims.Any())
            {
                return await UnauthorizeAsync();
            }

            var claims = tokenHandler.ReadClaims(rowToken);

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

            var rawToken = tokenHandler.ReadToken(accessToken)!;
            var claims = tokenHandler.ReadClaims(rawToken);
            claimsPrincipal = new(new ClaimsIdentity(claims, JwtBearerDefaults.AuthenticationScheme));

            var store = Map(claims);
            await userStorage.SaveAsync(store);
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


    private UserStore Map(List<Claim> claims)
    {
        var user = new UserStore
        {
            Id = Guid.Parse(claims.First(x => x.Type == AppClaimTypes.Id).Value),
            Email = claims.First(x => x.Type == AppClaimTypes.Email).Value,
            UserName = claims.First(x => x.Type == AppClaimTypes.UserName).Value,
            PhoneNumber = claims.FirstOrDefault(x => x.Type == AppClaimTypes.PhoneNumber)?.Value ?? null
        };

        return user;
    }
}