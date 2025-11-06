using System.Security.Claims;
using eSecurity.Common.JS.Fetch;
using eSecurity.Common.Responses;
using eSecurity.Common.Routing;
using eSecurity.Common.Storage;
using eSecurity.Security.Authentication.Jwt;
using eSecurity.Security.Authentication.Schemes;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace eSecurity.Security.Authentication;

public sealed class AuthenticationManager(
    NavigationManager navigationManager,
    AuthenticationStateProvider authenticationStateProvider,
    TokenProvider tokenProvider,
    IFetchClient fetchClient,
    IStorage storage)
{
    private readonly NavigationManager navigationManager = navigationManager;
    private readonly AuthenticationStateProvider authenticationStateProvider = authenticationStateProvider;
    private readonly TokenProvider tokenProvider = tokenProvider;
    private readonly IFetchClient fetchClient = fetchClient;
    private readonly IStorage storage = storage;

    public async Task SignInAsync()
    {
        var fetchOptions = new FetchOptions()
        {
            Url = $"{navigationManager.BaseUri}api/authentication/sign-in",
            Method = HttpMethod.Post,
        };

        var result = await fetchClient.FetchAsync(fetchOptions);
        if (result.Succeeded)
        {
            //TODO: Load user claims
            
            var claimsIdentity = new ClaimsIdentity([], AuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            (authenticationStateProvider as JwtAuthenticationStateProvider)!.SignIn(claimsPrincipal);
        }
    }

    public async Task SignOutAsync()
    {
        var fetchOptions = new FetchOptions()
        {
            Method = HttpMethod.Post,
            Url = $"{navigationManager.BaseUri}api/authentication/sign-out",
        };

        var result = await fetchClient.FetchAsync(fetchOptions);
        if (result.Succeeded)
        {
            await (authenticationStateProvider as JwtAuthenticationStateProvider)!.SignOutAsync();
            await storage.ClearAsync();
            tokenProvider.Clear();

            navigationManager.NavigateTo(Links.SignIn);
        }
    }
}