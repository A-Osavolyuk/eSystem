using eShop.Domain.Common.Security;

namespace eShop.Infrastructure.Security;

public class AuthenticationManager(
    AuthenticationStateProvider authenticationStateProvider,
    TokenHandler tokenHandler,
    ITokenProvider tokenProvider)
{
    private readonly AuthenticationStateProvider authenticationStateProvider = authenticationStateProvider;
    private readonly TokenHandler tokenHandler = tokenHandler;
    private readonly ITokenProvider tokenProvider = tokenProvider;

    public async Task LogInAsync(string accessToken, string refreshToken)
    {
        await (authenticationStateProvider as JwtAuthenticationStateProvider)!.LoginAsync(accessToken, refreshToken);
    }
    
    public async Task LogOutAsync()
    {
        await (authenticationStateProvider as JwtAuthenticationStateProvider)!.LogOutAsync();
    }
    
    public async Task ReauthenticateAsync(string accessToken, string refreshToken)
    {
        await (authenticationStateProvider as JwtAuthenticationStateProvider)!.ReauthenticateAsync(accessToken, refreshToken);
    }

    public async Task<JwtAuthenticationState> GetStateAsync()
    {
        var token = await tokenProvider.GetTokenAsync();
        var rawToken = tokenHandler.ReadToken(token);
        var claims = rawToken!.Claims.ToList();
        var state = new JwtAuthenticationState() { Claims = claims };
        
        return state;
    }
}