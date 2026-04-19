using System.Text.Json;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Session;
using eSecurity.Server.Security.Cookies;
using eSecurity.Server.Security.Cryptography.Protection.Constants;
using eSystem.Core.Enums;
using eSystem.Core.Primitives.Enums;
using eSystem.Core.Security.Authentication.OpenIdConnect;
using eSystem.Core.Security.Authentication.OpenIdConnect.Discovery;
using eSystem.Core.Utilities.Query;
using eSystem.Core.Utilities.State;
using Microsoft.AspNetCore.DataProtection;

namespace eSecurity.Server.Security.Authentication.OpenIdConnect.Prompt.Handlers;

public sealed class LoginPromptHandler(
    IHttpContextAccessor httpContextAccessor,
    IOptions<OpenIdConfiguration> options,
    IDataProtectionProvider protectionProvider,
    ISessionManager sessionManager) : IPromptHandler
{
    private readonly IDataProtectionProvider _protectionProvider = protectionProvider;
    private readonly ISessionManager _sessionManager = sessionManager;
    private readonly OpenIdConfiguration _configuration = options.Value;
    private readonly HttpContext _httpContext = httpContextAccessor.HttpContext!;

    public bool CanHandle(PromptType promptType) => promptType == PromptType.Login;

    public async ValueTask<PromptResult> HandleAsync(PromptContext context, CancellationToken cancellationToken)
    {
        if (_httpContext.Request.Cookies.TryGetValue(DefaultCookies.Session, out var cookie))
        {
            var protector = _protectionProvider.CreateProtector(ProtectionPurposes.Session);
            var unprotectedCookie = protector.Unprotect(cookie);
            try
            {
                var sessionCookie = JsonSerializer.Deserialize<SessionCookie>(unprotectedCookie);
                if (sessionCookie is not null)
                {
                    var session = await _sessionManager.FindByIdAsync(sessionCookie.SessionId, cancellationToken);
                    if (session is not null)
                        return PromptResult.Next();
                }
            }
            catch (Exception)
            {
                //TODO: Implement fallback
            }
        }

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