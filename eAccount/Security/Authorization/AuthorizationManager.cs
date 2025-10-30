using eAccount.Common.Routing;
using eAccount.Security.Authentication.SSO.Clients;
using eSystem.Core.Security.Authentication.SSO.Constants;
using eSystem.Core.Security.Cryptography.Keys;
using eSystem.Core.Utilities.Query;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;

namespace eAccount.Security.Authorization;

public class AuthorizationManager(
    NavigationManager navigationManager,
    IKeyFactory keyFactory,
    IOptions<ClientOptions> options)
{
    private readonly NavigationManager navigationManager = navigationManager;
    private readonly IKeyFactory keyFactory = keyFactory;
    private readonly IOptions<ClientOptions> options = options;

    public void Authorize()
    {
        var queryParams = QueryParser.GetQueryParameters(navigationManager.Uri);
        if (!queryParams.ContainsKey("client_id") && !queryParams.ContainsKey("redirect_uri"))
        {
            var clientOptions = options.Value;
            var state = keyFactory.Create(16);
            var nonce = keyFactory.Create(16);

            var builder = QueryBuilder.Create()
                .WithUri(Links.Authorize)
                .WithQueryParam("response_type", ResponseTypes.Code)
                .WithQueryParam("client_id", clientOptions.ClientId)
                .WithQueryParam("redirect_uri", clientOptions.RedirectUri)
                .WithQueryParam("scope", string.Join(' ', clientOptions.Scopes))
                .WithQueryParam("state", state)
                .WithQueryParam("nonce", nonce);

            if (queryParams.TryGetValue("return_url", out var returnUrl))
                builder.WithQueryParam("return_url", returnUrl);

            navigationManager.NavigateTo(builder.Build());
        }
        else
        {
            var builder = QueryBuilder.Create()
                .WithUri(Links.Authorize)
                .WithQueryParam("response_type", queryParams["response_type"])
                .WithQueryParam("client_id", queryParams["client_id"])
                .WithQueryParam("redirect_uri", queryParams["redirect_uri"])
                .WithQueryParam("scope", queryParams["scope"])
                .WithQueryParam("state", queryParams["state"])
                .WithQueryParam("nonce", queryParams["nonce"]);

            if (queryParams.TryGetValue("code_challenge", out var codeChallenge)
                && queryParams.TryGetValue("code_challenge_method", out var codeChallengeMethod))
            {
                builder.WithQueryParam("code_challenge", codeChallenge)
                    .WithQueryParam("code_challenge_method", codeChallengeMethod);
            }

            navigationManager.NavigateTo(builder.Build());
        }
    }
}