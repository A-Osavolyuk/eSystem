using eAccount.Blazor.Server.Application.State;
using eAccount.Blazor.Server.Domain.Options;
using eAccount.Blazor.Server.Domain.Requests;
using eSystem.Core.Requests.Auth;
using eSystem.Core.Security.Authentication.SSO;
using Microsoft.AspNetCore.Components;

namespace eAccount.Blazor.Server.Infrastructure.Security;

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
        var authorizeRequest = new AuthorizeRequest()
        {
            UserId = userState.UserId,
            ClientId = "client-sso",
            RedirectUri = "none",
            Scopes = [Scopes.OpenId, Scopes.Profile, Scopes.Email, Scopes.PhoneNumber, Scopes.Address],
            State = "State",
            Nonce = "Nonce"
        };
        var authorizeResponse = await AuthorizeAsync(authorizeRequest);
    }

    private async Task<HttpResponse> AuthorizeAsync(AuthorizeRequest request)
    {
        var fetchOptions = new FetchOptions()
        {
            Url = $"{navigationManager.BaseUri}api/auth/authorize",
            Method = HttpMethod.Post,
            Body = request
        };

        return await fetchClient.FetchAsync(fetchOptions);
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

    public async Task<HttpResponse> RefreshTokenAsync()
    {
        var request = new RefreshTokenRequest() { UserId = userState.UserId };
        var options = new FetchOptions()
        {
            Method = HttpMethod.Post,
            Url = $"{navigationManager.BaseUri}api/auth/refresh-token",
            Body = request
        };

        return await fetchClient.FetchAsync(options);
    }
}