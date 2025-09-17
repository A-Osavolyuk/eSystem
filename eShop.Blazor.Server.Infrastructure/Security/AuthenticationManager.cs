using eShop.Blazor.Server.Application.Routing;
using eShop.Blazor.Server.Domain.Interfaces;
using eShop.Domain.Responses.API.Auth;

namespace eShop.Blazor.Server.Infrastructure.Security;

public class AuthenticationManager(
    AuthenticationStateProvider authenticationStateProvider,
    RouteManager routeManager,
    ISecurityService securityService)
{
    private readonly AuthenticationStateProvider authenticationStateProvider = authenticationStateProvider;
    private readonly RouteManager routeManager = routeManager;

    public async Task SignInAsync()
    {
        var result = await securityService.AuthorizeAsync();

        if (result.Success)
        {
            var response = result.Get<AuthorizeResponse>()!;
            await (authenticationStateProvider as JwtAuthenticationStateProvider)!.SignInAsync(response.AccessToken);
        }
    }

    public async Task SignOutAsync()
    {
        var result = await securityService.UnauthorizeAsync();

        if (result.Success)
        {
            await (authenticationStateProvider as JwtAuthenticationStateProvider)!.SignOutAsync();
            routeManager.Route("/account/login");
        }
    }
}