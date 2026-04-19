using eSecurity.Server.Security.Authentication.OpenIdConnect.Session;
using eSystem.Core.Enums;
using eSystem.Core.Primitives.Enums;
using eSystem.Core.Security.Authentication.OpenIdConnect;
using eSystem.Core.Security.Authentication.OpenIdConnect.Discovery;
using eSystem.Core.Utilities.Query;
using eSystem.Core.Utilities.State;

namespace eSecurity.Server.Security.Authentication.OpenIdConnect.Prompt.Handlers;

public sealed class LoginPromptHandler(
    IOptions<OpenIdConfiguration> options,
    ISessionManager sessionManager,
    ISessionAccessor sessionAccessor) : IPromptHandler
{
    private readonly ISessionManager _sessionManager = sessionManager;
    private readonly ISessionAccessor _sessionAccessor = sessionAccessor;
    private readonly OpenIdConfiguration _configuration = options.Value;

    public bool CanHandle(PromptType promptType) => promptType == PromptType.Login;

    public async ValueTask<PromptResult> HandleAsync(PromptContext context, CancellationToken cancellationToken)
    {
        var cookie = _sessionAccessor.GetCookie();
        if (cookie is not null)
            return PromptResult.Next();

        var prompts = context.Prompts
            .Where(x => x != PromptType.Login)
            .Select(x => x.GetString())
            .ToList();
        
        if (prompts.Count == 0)
            prompts.Add(PromptType.None.GetString());
        
        var prompt = string.Join(' ', prompts);
        var scope = string.Join(' ', context.Scopes);
        var returnUrlBuilder = QueryBuilder.Create()
            .WithUri(_configuration.AuthorizationEndpoint)
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

        var state = StateBuilder.Create()
            .WithData("return_url", returnUrlBuilder.Build())
            .Build();

        var url = QueryBuilder.Create()
            .WithUri("https://localhost:6501/login")
            .WithQueryParam("state", state)
            .Build();

        return PromptResult.Success(
            Results.Redirect(RedirectionCode.Found, url)
        );
    }
}