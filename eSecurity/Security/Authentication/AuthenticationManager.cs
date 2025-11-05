using System.Security.Claims;
using eSecurity.Common.JS.Fetch;
using eSecurity.Common.Responses;
using eSecurity.Common.Routing;
using eSecurity.Common.Storage;
using eSecurity.Security.Authentication.Jwt;
using eSystem.Core.Requests.Auth;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using SignInRequest = eSecurity.Common.Requests.SignInRequest;

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

    public async Task<HttpResponse> RefreshAsync(string refreshToken)
    {
        var request = new RefreshRequest() { RefreshToken = refreshToken };
        var fetchOptions = new FetchOptions()
        {
            Method = HttpMethod.Post,
            Url = $"{navigationManager.BaseUri}api/authentication/refresh",
            Body = request
        };

        return await fetchClient.FetchAsync(fetchOptions);
    }

    public async Task SignInAsync(string accessToken, string refreshToken)
    {
        tokenProvider.AccessToken = accessToken;
        
        var request = new SignInRequest()
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };

        var fetchOptions = new FetchOptions()
        {
            Url = $"{navigationManager.BaseUri}api/authentication/sign-in",
            Method = HttpMethod.Post,
            Body = request
        };

        var result = await fetchClient.FetchAsync(fetchOptions);
        var response = result.Get<SignInResponse>()!;

        var identity = response.Identity;
        var claims = identity.Claims
            .Select(x => new Claim(x.Key, x.Value))
            .ToList();

        var claimsIdentity = new ClaimsIdentity(claims, identity.Scheme);
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        (authenticationStateProvider as JwtAuthenticationStateProvider)!.SignIn(claimsPrincipal);
    }

    public async Task SignOutAsync()
    {
        var fetchOptions = new FetchOptions()
        {
            Method = HttpMethod.Post,
            Url = $"{navigationManager.BaseUri}api/authentication/sign-out",
        };

        var result = await fetchClient.FetchAsync(fetchOptions);
        if (result.Success)
        {
            await (authenticationStateProvider as JwtAuthenticationStateProvider)!.SignOutAsync();
            await storage.ClearAsync();
            tokenProvider.Clear();

            navigationManager.NavigateTo(Links.SignIn);
        }
    }
}