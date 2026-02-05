using eSecurity.Client.Common.JS.Fetch;
using eSecurity.Core.Common.Routing;
using eSystem.Core.Security.Authentication.OpenIdConnect.Client;
using eSystem.Core.Security.Authentication.OpenIdConnect.Constants;
using eSystem.Core.Utilities.Query;
using eSystem.Core.Utilities.State;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Options;

namespace eSecurity.Client.Security.Authentication;

public sealed class AuthenticationManager(
    NavigationManager navigationManager,
    AuthenticationStateProvider authenticationStateProvider,
    IFetchClient fetchClient,
    IOptions<ClientOptions> clientOptions)
{
    private readonly NavigationManager _navigationManager = navigationManager;
    private readonly AuthenticationStateProvider _authenticationStateProvider = authenticationStateProvider;
    private readonly IFetchClient _fetchClient = fetchClient;
    private readonly ClientOptions _clientOptions = clientOptions.Value;
    
    public string GetAuthorizationUri(string? returnUrl = null)
    {
        var builder = QueryBuilder.Create().WithUri(Links.Connect.Authorize)
            .WithQueryParam("prompt", PromptTypes.Consent)
            .WithQueryParam("response_type", ResponseTypes.Code)
            .WithQueryParam("client_id", _clientOptions.ClientId)
            .WithQueryParam("redirect_uri", _clientOptions.CallbackUri)
            .WithQueryParam("scope", string.Join(" ", _clientOptions.SupportedScopes))
            .WithQueryParam("state", Guid.NewGuid().ToString())
            .WithQueryParam("nonce", Guid.NewGuid().ToString())
            .WithQueryParam("prompt", PromptTypes.Consent);

        if (!string.IsNullOrEmpty(returnUrl))
            builder.WithQueryParam("return_url", returnUrl);

        return builder.Build();
    }

    public string GetAuthorizationState(string? returnUrl = null)
    {
        return StateBuilder.Create()
            .WithData("return_url", GetAuthorizationUri(returnUrl))
            .Build();
    }

    public async Task SignOutAsync()
    {
        var fetchOptions = new FetchOptions
        {
            Method = HttpMethod.Post,
            Url = $"{_navigationManager.BaseUri}api/authentication/sign-out",
        };

        var result = await _fetchClient.FetchAsync(fetchOptions);
        if (result.Succeeded)
        {
            await (_authenticationStateProvider as ClaimAuthenticationStateProvider)!.SignOutAsync();
        }
    }

    public async Task LogoutAsync()
    {
        var fetchOptions = new FetchOptions
        {
            Method = HttpMethod.Post,
            Url = $"{_navigationManager.BaseUri}api/authentication/logout",
        };

        await _fetchClient.FetchAsync(fetchOptions);
    }

    public async Task RefreshAsync()
    {
        await (_authenticationStateProvider as ClaimAuthenticationStateProvider)!.NotifyAsync();
    }
}