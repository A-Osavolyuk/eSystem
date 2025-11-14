using eSecurity.Client.Common.JS.Fetch;
using eSecurity.Client.Common.Storage;
using eSecurity.Client.Security.Authentication.Oidc.Clients;
using eSecurity.Client.Security.Authentication.Oidc.Token;
using eSecurity.Core.Common.Routing;
using eSystem.Core.Security.Authentication.Oidc.Constants;
using eSystem.Core.Utilities.Query;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Options;

namespace eSecurity.Client.Security.Authentication;

public sealed class AuthenticationManager(
    NavigationManager navigationManager,
    AuthenticationStateProvider authenticationStateProvider,
    TokenProvider tokenProvider,
    IFetchClient fetchClient,
    IStorage storage,
    IOptions<ClientOptions> clientOptions)
{
    private readonly NavigationManager _navigationManager = navigationManager;
    private readonly AuthenticationStateProvider _authenticationStateProvider = authenticationStateProvider;
    private readonly IFetchClient _fetchClient = fetchClient;
    private readonly IStorage _storage = storage;
    private readonly ClientOptions _clientOptions = clientOptions.Value;

    public void Authorize()
    {
        var redirectUri = QueryBuilder.Create()
            .WithUri("/connect/authorize")
            .WithQueryParam("response_type", ResponseTypes.Code)
            .WithQueryParam("client_id", _clientOptions.ClientId)
            .WithQueryParam("redirect_uri", _clientOptions.CallbackUri)
            .WithQueryParam("scope", string.Join(" ", _clientOptions.Scopes))
            .WithQueryParam("nonce", "1234567890")
            .WithQueryParam("state", "1234567890")
            .WithQueryParam("prompt", Prompts.Consent)
            .Build();
        
        _navigationManager.NavigateTo(redirectUri);
    }

    public async Task SignOutAsync()
    {
        var fetchOptions = new FetchOptions()
        {
            Method = HttpMethod.Post,
            Url = $"{_navigationManager.BaseUri}api/authentication/sign-out",
        };

        var result = await _fetchClient.FetchAsync(fetchOptions);
        if (result.Success)
        {
            await (_authenticationStateProvider as ClaimAuthenticationStateProvider)!.SignOutAsync();
            await _storage.ClearAsync();

            _navigationManager.NavigateTo(Links.Account.SignIn);
        }
    }
}