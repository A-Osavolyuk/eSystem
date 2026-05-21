using eSecurity.Idp.Security.Authentication.OpenIdConnect.Prompt;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;

namespace eSecurity.Idp.Security.Authorization.Authorize.Par;

public sealed class ParAuthorizationFlowHandler(
    IParManager parManager,
    IPromptsProcessor promptsProcessor) : IAuthorizationFlowHandler
{
    private readonly IParManager _parManager = parManager;
    private readonly IPromptsProcessor _promptsProcessor = promptsProcessor;

    public async ValueTask<Result> HandleAsync(AuthorizationRequest request, 
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(request.RequestUri))
        {
            return Results.ServerError(ServerErrorCode.InternalServerError, new Error()
            {
                Code = ErrorCode.ServerError,
                Description = "Server error"
            });
        }

        var par = await _parManager.FindByRequestUriAsync(request.RequestUri, cancellationToken);
        if (par is null)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.InvalidRequestUri,
                Description = "request_uri is invalid"
            });
        }

        if (par.Status is ParState.Cancelled or ParState.Consumed or ParState.Expired)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.InvalidRequestUri,
                Description = "invalid request_uri"
            });
        }

        if (par.ExpiredAt > DateTimeOffset.UtcNow)
        {
            par.Status = ParState.Expired;
            
            var updateResult = await _parManager.UpdateAsync(par, cancellationToken);
            if (!updateResult.Succeeded) 
                return updateResult;
            
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.InvalidRequestUri,
                Description = "invalid request_uri"
            });
        }

        var promptContext = new PromptContext()
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