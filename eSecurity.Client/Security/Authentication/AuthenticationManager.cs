using System.Security.Claims;
using eSecurity.Client.Common.JS.Fetch;
using eSecurity.Client.Common.State.States;
using eSecurity.Client.Common.Storage;
using eSecurity.Client.Security.Authentication.Odic.Token;
using eSecurity.Core.Common.DTOs;
using eSecurity.Core.Common.Routing;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace eSecurity.Client.Security.Authentication;

public sealed class AuthenticationManager(
    NavigationManager navigationManager,
    AuthenticationStateProvider authenticationStateProvider,
    UserState userState,
    TokenProvider tokenProvider,
    IFetchClient fetchClient,
    IStorage storage)
{
    private readonly NavigationManager navigationManager = navigationManager;
    private readonly AuthenticationStateProvider authenticationStateProvider = authenticationStateProvider;
    private readonly TokenProvider tokenProvider = tokenProvider;
    private readonly UserState userState = userState;
    private readonly IFetchClient fetchClient = fetchClient;
    private readonly IStorage storage = storage;

    public void Authorize()
    {
        //TODO: Implement OIDC authorization flow initialization
    }
    
    public async Task SignInAsync(string refreshToken, string accessToken, string idToken)
    {
        tokenProvider.AccessToken = accessToken;
        tokenProvider.IdToken = idToken;
        
        var tokenIdentity = new TokenIdentity()
        {
            RefreshToken = refreshToken, 
            IdToken = idToken
        };
        
        var fetchOptions = new FetchOptions()
        {
            Url = $"{navigationManager.BaseUri}api/authentication/sign-in",
            Method = HttpMethod.Post,
            Body = tokenIdentity
        };

        var result = await fetchClient.FetchAsync(fetchOptions);
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
            await (authenticationStateProvider as ClaimAuthenticationStateProvider)!.SignOutAsync();
            await storage.ClearAsync();

            navigationManager.NavigateTo(Links.Account.SignIn);
        }
    }
}