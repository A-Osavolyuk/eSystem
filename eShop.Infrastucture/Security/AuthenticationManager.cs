namespace eShop.Infrastructure.Security;

public class AuthenticationManager(AuthenticationStateProvider authenticationStateProvider)
{
    private readonly AuthenticationStateProvider authenticationStateProvider = authenticationStateProvider;

    public async Task SignInAsync(string accessToken, string refreshToken)
    {
        await (authenticationStateProvider as JwtAuthenticationStateProvider)!.SignInAsync(accessToken, refreshToken);
    }

    public async Task SignOutAsync()
    {
        await (authenticationStateProvider as JwtAuthenticationStateProvider)!.SignOutAsync();
    }
}