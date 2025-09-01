using eShop.Domain.Common.Security;
using eShop.Domain.Requests.API.Auth;
using eShop.Domain.Responses.API.Auth;
using eShop.Infrastructure.State;

namespace eShop.Infrastructure.Security;

public class JwtAuthenticationStateProvider(
    ITokenProvider tokenProvider,
    IStorage storage,
    ISecurityService securityService,
    TokenHandler tokenHandler,
    UserStateContainer userState) : AuthenticationStateProvider
{
    private readonly AuthenticationState anonymous = new(new ClaimsPrincipal());
    private readonly ITokenProvider tokenProvider = tokenProvider;
    private readonly IStorage storage = storage;
    private readonly ISecurityService securityService = securityService;
    private readonly TokenHandler tokenHandler = tokenHandler;
    private readonly UserStateContainer userState = userState;

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var token = await tokenProvider.GetAsync();

            if (string.IsNullOrEmpty(token))
            {
                return await UnauthorizeAsync();
            }

            var valid = tokenHandler.Validate(token);

            if (!valid)
            {
                return await RefreshTokenAsync(token);
            }

            var rawToken = tokenHandler.ReadToken(token);

            if (rawToken is null || !rawToken.Claims.Any())
            {
                return await UnauthorizeAsync();
            }

            var claims = rawToken.Claims.ToList();
            var userId = Guid.Parse(rawToken.Claims.Single(x => x.Type == AppClaimTypes.Subject).Value);
            userState.UserId = userId;
            
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
            await tokenProvider.SetAsync(refreshToken);
            var authenticationState = Authorized(refreshToken);

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
            await tokenProvider.SetAsync(refreshToken);

            var rawToken = tokenHandler.ReadToken(accessToken)!;
            var claims = rawToken.Claims.ToList();
            var identity = new ClaimsIdentity(claims, JwtBearerDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(identity);
            var authenticationState = new AuthenticationState(claimsPrincipal);

            var userId = Guid.Parse(claims.First(x => x.Type == AppClaimTypes.Subject).Value);
            var jti = Guid.Parse(claims.First(x => x.Type == AppClaimTypes.Jti).Value);
            var exp = long.Parse(claims.First(x => x.Type == AppClaimTypes.Exp).Value);
            
            await storage.SetAsync("userId", userId);
            await storage.SetAsync("jti", jti);
            await storage.SetAsync("exp", exp);

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
        await storage.ClearAsync();
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

    private async Task<AuthenticationState> RefreshTokenAsync(string token)
    {
        var rawToken = tokenHandler.ReadToken(token)!;
        var claims = rawToken.Claims.ToList();
        var userId = Guid.Parse(claims.First(x => x.Type == AppClaimTypes.Subject).Value);

        var request = new RefreshTokenRequest()
        {
            Token = token,
            UserId = userId
        };

        var result = await securityService.RefreshTokenAsync(request);

        if (!result.Success)
        {
            return await LogOutAsync();
        }
        
        var response = result.Get<RefreshTokenResponse>()!;
        
        return Authorized(response.RefreshToken);
    }

    private AuthenticationState Authorized(string token)
    {
        var rawToken = tokenHandler.ReadToken(token)!;
        var claims = rawToken.Claims.ToList();
        var identity = new ClaimsIdentity(claims, JwtBearerDefaults.AuthenticationScheme);
        var claimsPrincipal = new ClaimsPrincipal(identity);
        var authenticationState = new AuthenticationState(claimsPrincipal);
        
        return authenticationState;
    }
}