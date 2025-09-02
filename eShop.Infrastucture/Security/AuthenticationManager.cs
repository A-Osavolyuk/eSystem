namespace eShop.Infrastructure.Security;

public class AuthenticationManager(AuthenticationStateProvider authenticationStateProvider)
{
    private readonly AuthenticationStateProvider authenticationStateProvider = authenticationStateProvider;

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
        await (authenticationStateProvider as JwtAuthenticationStateProvider)!.ReauthenticateAsync(accessToken,
            refreshToken);
    }
}