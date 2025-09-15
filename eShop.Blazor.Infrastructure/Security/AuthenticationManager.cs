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
    UserState state,
    IFetchClient fetchClient)
{
    private readonly AuthenticationStateProvider authenticationStateProvider = authenticationStateProvider;
    private readonly RouteManager routeManager = routeManager;
    private readonly UserState state = state;
    private readonly IFetchClient fetchClient = fetchClient;

    public async Task SignInAsync()
    {
        var result = await AuthorizeAsync();

        if (result.Success)
        {
            var response = result.Get<AuthorizeResponse>()!;
            await (authenticationStateProvider as JwtAuthenticationStateProvider)!.SignInAsync(response.AccessToken);
        }
    }

    public async Task SignOutAsync()
    {
        var result = await UnauthorizeAsync();

        if (result.Success)
        {
            await (authenticationStateProvider as JwtAuthenticationStateProvider)!.SignOutAsync();
            routeManager.Route("/account/login");
        }
    }

    private async Task<HttpResponse> AuthorizeAsync()
    {
        var request = new AuthorizeRequest() { UserId = state.UserId };
        var body = JsonSerializer.Serialize(request);

        var options = new FetchOptions()
        {
            Url = "/api/v1/Security/authorize",
            Method = HttpMethod.Post,
            Credentials = Credentials.Include,
            Body = body
        };

        return await fetchClient.FetchAsync(options);
    }

    private async Task<HttpResponse> UnauthorizeAsync()
    {
        var request = new UnauthorizeRequest() { UserId = state.UserId };
        var body = JsonSerializer.Serialize(request);

        var options = new FetchOptions()
        {
            Url = "/api/v1/Security/unauthorize",
            Method = HttpMethod.Post,
            Credentials = Credentials.Include,
            Body = body
        };

        return await fetchClient.FetchAsync(options);
    }
}