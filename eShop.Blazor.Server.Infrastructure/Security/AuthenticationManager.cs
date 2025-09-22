using System.Text.Json;
using eShop.Blazor.Server.Application.Routing;
using eShop.Blazor.Server.Domain.Interfaces;
using eShop.Blazor.Server.Domain.Options;
using eShop.Blazor.Server.Domain.Types;
using eShop.Domain.Common.Http;
using eShop.Domain.Responses.API.Auth;

namespace eShop.Blazor.Server.Infrastructure.Security;

public class AuthenticationManager(
    AuthenticationStateProvider authenticationStateProvider,
    RouteManager routeManager,
    IFetchClient fetchClient,
    ISecurityService securityService)
{
    private readonly AuthenticationStateProvider authenticationStateProvider = authenticationStateProvider;
    private readonly RouteManager routeManager = routeManager;
    private readonly IFetchClient fetchClient = fetchClient;
    private readonly ISecurityService securityService = securityService;

    public async Task SignInAsync()
    {
        var result = await securityService.AuthorizeAsync();

        if (result.Success)
        {
            var response = result.Get<AuthorizeResponse>()!;
            var request = new AuthenticationRequest() { AccessToken = response.AccessToken };
            var body = JsonSerializer.Serialize(request);
            var fetchOptions = new FetchOptions()
            {
                Method = HttpMethod.Post,
                Url = $"{routeManager.BaseUri}api/auth/sign-in",
                Credentials = Credentials.Include,
                Body = body
            };
            
            await fetchClient.FetchAsync(fetchOptions);
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