using eShop.Domain.Common.Security;

namespace eShop.Infrastructure.Security;

public class JwtAuthenticationStateProvider(
    ITokenProvider tokenProvider,
    IStorage localStorage,
    TokenHandler tokenHandler) : AuthenticationStateProvider
{
    private readonly AuthenticationState anonymous = new(new ClaimsPrincipal());
    private readonly ITokenProvider tokenProvider = tokenProvider;
    private readonly IStorage localStorage = localStorage;
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

    public async Task ReauthenticateAsync(string accessToken, string refreshToken)
    {
        if (!string.IsNullOrEmpty(accessToken) && !string.IsNullOrEmpty(refreshToken))
        {
            await tokenProvider.SetTokenAsync(refreshToken);

            var rawToken = tokenHandler.ReadToken(accessToken)!;
            var claims = tokenHandler.ReadClaims(rawToken);
            var identity = new ClaimsIdentity(claims, JwtBearerDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(identity);
            var authenticationState = new AuthenticationState(claimsPrincipal);
            
            NotifyAuthenticationStateChanged(Task.FromResult(authenticationState));
        }
        else
        {
            await UnauthorizeAsync();
        }
    }

    public async Task LoginAsync(string accessToken, string refreshToken)
    {
        if (!string.IsNullOrEmpty(accessToken) && !string.IsNullOrEmpty(refreshToken))
        {
            await tokenProvider.SetTokenAsync(refreshToken);

            var rawToken = tokenHandler.ReadToken(accessToken)!;
            var claims = tokenHandler.ReadClaims(rawToken);
            var identity = new ClaimsIdentity(claims, JwtBearerDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(identity);
            var authenticationState = new AuthenticationState(claimsPrincipal);

            var userId = Map(claims);
            await localStorage.SetAsync("userId", userId);
            
            NotifyAuthenticationStateChanged(Task.FromResult(authenticationState));
        }
        else
        {
            await UnauthorizeAsync();
        }
    }

    public async Task<AuthenticationState> LogOutAsync()
    {
        await tokenProvider.RemoveAsync();
        await localStorage.ClearAsync();
        return await UnauthorizeAsync();
    }

    private async Task<AuthenticationState> UnauthorizeAsync()
    {
        try
        {
            NotifyAuthenticationStateChanged(Task.FromResult(anonymous));
            return await Task.FromResult(anonymous);
        }
        catch (Exception)
        {
            return await Task.FromResult(anonymous);
        }
    }


    private Guid Map(List<Claim> claims)
    {
        var userId = Guid.Parse(claims.First(x => x.Type == AppClaimTypes.Id).Value);
        return userId;
    }
}