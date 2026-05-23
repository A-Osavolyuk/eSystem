using eSecurity.Idp.Security.Authorization.Authorize;
using eSystem.Core.Enums;
using eSystem.Core.Server.Security.Authentication.OpenIdConnect.Discovery;
using eSystem.Core.Utilities.Query;
using eSystem.Core.Utilities.State;

namespace eSecurity.Idp.Security.Authentication.OpenIdConnect.Prompt;

public sealed class PromptStateFactory(IOptions<OpenIdConfiguration> options) : IPromptStateFactory
{
    private readonly OpenIdConfiguration _configuration = options.Value;

    public string CreateState(PromptContext context)
    {
        var prompt = string.Join(' ', context.Prompts.Select(x => x.GetString()));
        var scope = string.Join(' ', context.Scopes);
        var returnUrlBuilder = QueryBuilder.Create()
            .WithUri(_configuration.AuthorizationEndpoint);

        if (context.AuthorizationFlow == AuthorizationFlow.Manual)
        {
            returnUrlBuilder
                .WithQueryParam("response_type", context.ResponseType)
                .WithQueryParam("client_id", context.ClientId)
                .WithQueryParam("redirect_uri", context.RedirectUri)
                .WithQueryParam("scope", scope)
                .WithQueryParam("prompt", prompt);

            if (!string.IsNullOrEmpty(context.State))
                returnUrlBuilder.WithQueryParam("state", context.State);

            if (!string.IsNullOrEmpty(context.Nonce))
                returnUrlBuilder.WithQueryParam("nonce", context.Nonce);

            if (!string.IsNullOrEmpty(context.CodeChallenge) && context.CodeChallengeMethod is not null)
            {
                returnUrlBuilder.WithQueryParam("code_challenge", context.CodeChallenge);
                returnUrlBuilder.WithQueryParam("code_challenge_method", context.CodeChallengeMethod.Value);
            }
        }
        else if (context.AuthorizationFlow == AuthorizationFlow.PushedAuthorizationRequest)
        {
            if (string.IsNullOrEmpty(context.RequestUri))
                throw new NotSupportedException("RequestUri cannot be null or empty while PAR flow");

            returnUrlBuilder
                .WithQueryParam("request_uri", context.RequestUri)
                .WithQueryParam("client_id", context.ClientId);
        }
        else
        {
            if (string.IsNullOrEmpty(context.Request))
                throw new NotSupportedException("Request cannot be null or empty while JAR flow");

            returnUrlBuilder
                .WithQueryParam("request", context.Request)
                .WithQueryParam("client_id", context.ClientId);
        }

        return StateBuilder.Create()
            .WithData("return_url", returnUrlBuilder.Build())
            .Build();
    }
}