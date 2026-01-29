using System.Web;
using eSecurity.Client.Security.Authentication.OpenIdConnect.Authorization;
using eSecurity.Core.Common.Routing;
using eSystem.Core.Utilities.Query;
using eSystem.Core.Utilities.State;

namespace eSecurity.Client.Security.Authentication.OpenIdConnect.Prompts;

public static class PromptUtils
{
    public static string GetRedirectUri(string uri, AuthorizationContext context)
    {
        var decodedRedirectUri = HttpUtility.UrlDecode(context.RedirectUri);
        var decodedScope = HttpUtility.UrlDecode(context.Scope);
        
        var returnUrlBuilder = QueryBuilder.Create()
            .WithUri(Links.Connect.Authorize)
            .WithQueryParam("response_type", context.ResponseType)
            .WithQueryParam("client_id", context.ClientId)
            .WithQueryParam("redirect_uri", decodedRedirectUri)
            .WithQueryParam("scope", decodedScope)
            .WithQueryParam("state", context.State)
            .WithQueryParam("nonce", context.Nonce);

        if (!string.IsNullOrEmpty(context.CodeChallenge))
            returnUrlBuilder.WithQueryParam("code_challenge", context.CodeChallenge);

        if (!string.IsNullOrEmpty(context.CodeChallengeMethod))
            returnUrlBuilder.WithQueryParam("code_challenge_method", context.CodeChallengeMethod);

        if (context.Prompts.Count > 1)
        {
            var currentPrompt = context.Prompts.First();
            var prompt = string.Join(" ", context.Prompts.Except([currentPrompt]));
            returnUrlBuilder.WithQueryParam("prompt", prompt);
        }

        var state = StateBuilder.Create()
            .WithData("return_url", returnUrlBuilder.Build())
            .Build();

        return QueryBuilder.Create().WithUri(uri)
            .WithQueryParam("state", state)
            .Build();
    }
}