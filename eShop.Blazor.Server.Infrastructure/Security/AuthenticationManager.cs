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
using Microsoft.AspNetCore.Authentication.Cookies;

namespace eShop.Blazor.Server.Infrastructure.Security;

public class AuthenticationManager(
    AuthenticationStateProvider authenticationStateProvider,
    RouteManager routeManager,
    UserState userState,
    IFetchClient fetchClient,
    ISecurityService securityService)
{
    private readonly AuthenticationStateProvider authenticationStateProvider = authenticationStateProvider;
    private readonly RouteManager routeManager = routeManager;
    private readonly UserState userState = userState;
    private readonly IFetchClient fetchClient = fetchClient;
    private readonly ISecurityService securityService = securityService;

    public async Task SignInAsync()
    {
        var request = new RefreshTokenRequest { UserId = userState.UserId };
        var result = await securityService.RefreshTokenAsync(request);

        if (result.Success)
        {
            var response = result.Get<AuthorizeResponse>()!;
            var authRequest = new AuthenticationRequest() { AccessToken = response.AccessToken };
            var body = JsonSerializer.Serialize(authRequest);
            var fetchOptions = new FetchOptions()
            {
                Method = HttpMethod.Post,
                Url = $"{routeManager.BaseUri}api/auth/sign-in",
                Credentials = Credentials.Include,
                Body = body
            };

            var authResult = await fetchClient.FetchAsync(fetchOptions);

            var identity = authResult.Get<ClaimIdentityDto>()!;
            var claims = identity.Claims.Select(x => new Claim(x.Type, x.Value)).ToList();
            var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, identity.Scheme));
            
            await (authenticationStateProvider as JwtAuthenticationStateProvider)!.SignInAsync(principal);
        }
    }

    public Task SignOutAsync()
    {
        //TODO: Implement user sign-out
        return Task.CompletedTask;
    }
}