using System.Text.Json;
using eShop.Blazor.Server.Application.Routing;
using eShop.Blazor.Server.Application.State;
using eShop.Blazor.Server.Domain.DTOs;
using eShop.Blazor.Server.Domain.Interfaces;
using eShop.Blazor.Server.Domain.Options;
using eShop.Blazor.Server.Domain.Types;
using eShop.Domain.Common.Http;
using eShop.Domain.Requests.API.Auth;
using eShop.Domain.Responses.API.Auth;

namespace eShop.Blazor.Server.Infrastructure.Security;

public class AuthenticationManager(
    AuthenticationStateProvider authenticationStateProvider,
    RouteManager routeManager,
    UserState userState,
    TokenProvider tokenProvider,
    IFetchClient fetchClient,
    IStorage storage,
    ISecurityService securityService)
{
    private readonly AuthenticationStateProvider authenticationStateProvider = authenticationStateProvider;
    private readonly RouteManager routeManager = routeManager;
    private readonly UserState userState = userState;
    private readonly TokenProvider tokenProvider = tokenProvider;
    private readonly IFetchClient fetchClient = fetchClient;
    private readonly IStorage storage = storage;
    private readonly ISecurityService securityService = securityService;

    public async Task SignInAsync()
    {
        var request = new AuthorizeRequest { UserId = userState.UserId };
        var result = await securityService.AuthorizeAsync(request);
        if (result.Success)
        {
            var response = result.Get<AuthorizeResponse>()!;
            tokenProvider.AccessToken = response.AccessToken;
            
            var authRequest = new SignInRequest() { AccessToken = tokenProvider.AccessToken };
            var body = JsonSerializer.Serialize(authRequest);
            var fetchOptions = new FetchOptions()
            {
                Method = HttpMethod.Post,
                Url = $"{routeManager.BaseUri}api/auth/sign-in",
                Credentials = Credentials.Include,
                Body = body
            };

            var authResult = await fetchClient.FetchAsync(fetchOptions);
            if (authResult.Success)
            {
                var identity = authResult.Get<ClaimsIdentityDto>()!;
                var claims = identity.Claims.Select(x => new Claim(x.Type, x.Value)).ToList();
                var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, identity.Scheme));
            
                await (authenticationStateProvider as JwtAuthenticationStateProvider)!.SignInAsync(principal);
            }
        }
    }

    public async Task SignOutAsync()
    {
        var request = new UnauthorizeRequest(){ UserId = userState.UserId };
        var result =  await securityService.UnauthorizeAsync(request);
        if (result.Success)
        {
            var fetchOptions = new FetchOptions()
            {
                Method = HttpMethod.Post,
                Url = $"{routeManager.BaseUri}api/auth/sign-out",
                Credentials = Credentials.Include,
            };

            var authResult = await fetchClient.FetchAsync(fetchOptions);
            if (authResult.Success)
            {
                await (authenticationStateProvider as JwtAuthenticationStateProvider)!.SignOutAsync();

                await storage.ClearAsync();
                tokenProvider.Clear();
                
                routeManager.Route("/account/login");
            }
        }
    }
}