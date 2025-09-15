namespace eShop.Blazor.Infrastructure.Security;

public class AuthenticationManager(AuthenticationStateProvider authenticationStateProvider)
{
    private readonly AuthenticationStateProvider authenticationStateProvider = authenticationStateProvider;

    public async Task SignInAsync(string accessToken)
    {
        await (authenticationStateProvider as JwtAuthenticationStateProvider)!.SignInAsync(accessToken);
    }

    public async Task SignOutAsync()
    {
        await (authenticationStateProvider as JwtAuthenticationStateProvider)!.SignOutAsync();
    }
}