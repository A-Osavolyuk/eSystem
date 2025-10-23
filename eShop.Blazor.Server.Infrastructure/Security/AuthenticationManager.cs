using eShop.Blazor.Server.Application.State;
using eShop.Blazor.Server.Domain.Interfaces;
using eShop.Blazor.Server.Domain.Options;
using eShop.Blazor.Server.Domain.Requests;
using eShop.Blazor.Server.Domain.Responses;
using eShop.Domain.Common.Http;
using Microsoft.AspNetCore.Components;

namespace eShop.Blazor.Server.Infrastructure.Security;

public class AuthenticationManager(
    AuthenticationStateProvider authenticationStateProvider,
    UserState userState,
    TokenProvider tokenProvider,
    NavigationManager navigationManager,
    IFetchClient fetchClient,
    IStorage storage)
{
    private readonly AuthenticationStateProvider authenticationStateProvider = authenticationStateProvider;
    private readonly UserState userState = userState;
    private readonly TokenProvider tokenProvider = tokenProvider;
    private readonly NavigationManager navigationManager = navigationManager;
    private readonly IFetchClient fetchClient = fetchClient;
    private readonly IStorage storage = storage;

    public async Task SignInAsync()
    {
        var request = new SignInRequest() { UserId = userState.UserId };
        var fetchOptions = new FetchOptions()
        {
            Method = HttpMethod.Post,
            Url = $"{navigationManager.BaseUri}api/auth/sign-in",
            Body = request
        };

        var result = await fetchClient.FetchAsync(fetchOptions);
        if (result.Success)
        {
            var response = result.Get<SignInResponse>()!;
            tokenProvider.AccessToken = response.AccessToken;

            var identity = response.Identity;
            var claims = identity.Claims.Select(x => new Claim(x.Type, x.Value)).ToList();
            var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, identity.Scheme));

            await (authenticationStateProvider as JwtAuthenticationStateProvider)!.SignInAsync(principal);
        }
    }

    public async Task SignOutAsync()
    {
        var request = new SignOutRequest()
        {
            UserId = userState.UserId,
            AccessToken = tokenProvider.AccessToken!
        };
        
        var fetchOptions = new FetchOptions()
        {
            Method = HttpMethod.Post,
            Url = $"{navigationManager.BaseUri}api/auth/sign-out",
            Body = request
        };

        var authResult = await fetchClient.FetchAsync(fetchOptions);
        if (authResult.Success)
        {
            await (authenticationStateProvider as JwtAuthenticationStateProvider)!.SignOutAsync();

            await storage.ClearAsync();
            tokenProvider.Clear();

            navigationManager.NavigateTo("/account/login");
        }
    }
}