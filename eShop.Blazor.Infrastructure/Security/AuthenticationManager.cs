using eShop.Blazor.Application.Routing;

namespace eShop.Blazor.Infrastructure.Security;

public class AuthenticationManager(
    AuthenticationStateProvider authenticationStateProvider,
    RouteManager routeManager)
{
    private readonly AuthenticationStateProvider authenticationStateProvider = authenticationStateProvider;
    private readonly RouteManager routeManager = routeManager;

    public async Task SignInAsync(string accessToken)
    {
        await (authenticationStateProvider as JwtAuthenticationStateProvider)!.SignInAsync(accessToken);
    }

    public async Task SignOutAsync()
    {
        await (authenticationStateProvider as JwtAuthenticationStateProvider)!.SignOutAsync();
        routeManager.Route("/account/login");
    }

    public async Task UnauthorizedAsync()
    {
        await (authenticationStateProvider as JwtAuthenticationStateProvider)!.SignOutAsync();
        routeManager.Route("/error?code=401");
    }
}