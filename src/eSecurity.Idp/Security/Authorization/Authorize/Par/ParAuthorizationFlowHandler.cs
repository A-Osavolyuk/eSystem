using eSecurity.Idp.Security.Authorization.Prompt;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;

namespace eSecurity.Idp.Security.Authorization.Authorize.Par;

public sealed class ParAuthorizationFlowHandler(
    IParManager parManager,
    IPromptsProcessor promptsProcessor,
    RedirectManager redirectManager) : IAuthorizationFlowHandler
{
    private readonly IParManager _parManager = parManager;
    private readonly IPromptsProcessor _promptsProcessor = promptsProcessor;
    private readonly RedirectManager _redirectManager = redirectManager;

    public async ValueTask<Result> HandleAsync(AuthorizationRequest request, 
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(request.RequestUri))
        {
            var uri = _redirectManager.GetRedirectUri(ErrorCode.ServerError, "Server error");
            return Results.Redirect(RedirectionCode.Found, uri);
        }

        var par = await _parManager.FindByRequestUriAsync(request.RequestUri, cancellationToken);
        if (par is null)
        {
            var uri = _redirectManager.GetRedirectUri(ErrorCode.InvalidRequestUri, 
                "request_uri is invalid");
            
            return Results.Redirect(RedirectionCode.Found, uri);
        }

        if (par.Status is ParState.Cancelled or ParState.Consumed or ParState.Expired)
        {
            var uri = _redirectManager.GetRedirectUri(ErrorCode.InvalidRequestUri, 
                "request_uri is invalid");
            
            return Results.Redirect(RedirectionCode.Found, uri);
        }

        if (DateTimeOffset.UtcNow > par.ExpiredAt)
        {
            par.Status = ParState.Expired;
            
            var updateResult = await _parManager.UpdateAsync(par, cancellationToken);
            if (!updateResult.Succeeded) 
                return updateResult;
            
            var uri = _redirectManager.GetRedirectUri(ErrorCode.InvalidRequestUri, 
                "request_uri is invalid");
            
            return Results.Redirect(RedirectionCode.Found, uri);
        }

        var promptContext = new PromptContext
        {
            AuthorizationFlow = AuthorizationFlow.PushedAuthorizationRequest,
            ClientId = par.ClientId,
            ResponseType = par.ResponseType,
            RedirectUri = par.RedirectUri,
            RequestUri = par.RequestUri,
            State = par.State,
            Nonce = par.Nonce,
            CodeChallenge = par.CodeChallenge,
            CodeChallengeMethod = par.CodeChallengeMethod,
            Prompts = par.Prompts.Select(x => x.Prompt).ToList(),
            Scopes = par.Scopes.Select(x => x.Scope).ToList()
        };

        return await _promptsProcessor.ProcessAsync(promptContext, cancellationToken);
    }
}