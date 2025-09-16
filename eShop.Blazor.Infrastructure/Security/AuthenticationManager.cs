using System.Text.Json;
using eShop.Blazor.Application.Routing;
using eShop.Blazor.Application.State;
using eShop.Blazor.Domain.Interfaces;
using eShop.Blazor.Domain.Options;
using eShop.Blazor.Infrastructure.Implementations;
using eShop.Domain.Common.Http;
using eShop.Domain.Requests.API.Auth;
using eShop.Domain.Responses.API.Auth;
using Microsoft.JSInterop;

namespace eShop.Blazor.Infrastructure.Security;

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