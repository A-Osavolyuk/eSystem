using System.Web;
using eSecurity.Client.Common.JS.Fetch;
using eSecurity.Core.Common.Routing;
using eSystem.Core.Security.Authentication.Oidc.Client;
using eSystem.Core.Security.Authentication.Oidc.Constants;
using eSystem.Core.Utilities.Query;
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

    public void Authorize()
    {
        var decodedUri = HttpUtility.UrlDecode(_navigationManager.Uri);
        var queryParams = QueryParser.GetQueryParameters(decodedUri);

        var builder = QueryBuilder.Create().WithUri(Links.Connect.Authorize)
            .WithQueryParam("prompt", Prompts.Consent);

        builder.WithQueryParam("response_type", queryParams.GetValueOrDefault("response_type", ResponseTypes.Code))
            .WithQueryParam("client_id", queryParams.GetValueOrDefault("client_id", _clientOptions.ClientId))
            .WithQueryParam("redirect_uri", queryParams.GetValueOrDefault("redirect_uri", _clientOptions.CallbackUri))
            .WithQueryParam("scope", queryParams.GetValueOrDefault("scope", string.Join(" ", _clientOptions.SupportedScopes)))
            .WithQueryParam("state", queryParams.GetValueOrDefault("state", Guid.NewGuid().ToString()))
            .WithQueryParam("nonce", queryParams.GetValueOrDefault("nonce", Guid.NewGuid().ToString()));

        if (queryParams.TryGetValue("prompt", out var prompt))
        {
            var prompts = prompt.Split(' ');
            if (prompts.Length > 1)
            {
                builder.WithQueryParam("prompt", string.Join(" ", prompts.Skip(1)));
            }
        }
        else 
            builder.WithQueryParam("prompt", Prompts.Consent);

        _navigationManager.NavigateTo(builder.Build());
    }

    public async Task SignOutAsync()
    {
        var fetchOptions = new FetchOptions()
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
        var fetchOptions = new FetchOptions()
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