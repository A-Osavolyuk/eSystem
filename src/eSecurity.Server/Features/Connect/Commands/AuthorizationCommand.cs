using eSecurity.Core.Common.Requests;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Client;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Prompt;
using eSecurity.Server.Security.Authorization.OAuth.Scopes;
using eSystem.Core.Enums;
using eSystem.Core.Mediator;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;
using eSystem.Core.Security.Authentication.OpenIdConnect;
using eSystem.Core.Security.Authentication.OpenIdConnect.Discovery;
using eSystem.Core.Utilities.Query;

namespace eSecurity.Server.Features.Connect.Commands;

public sealed record AuthorizationCommand(AuthorizationRequest Request) : IRequest<Result>;

public sealed class AuthorizationCommandHandler(
    IClientManager clientManager,
    IOptions<OpenIdConfiguration> options,
    IEnumerable<IPromptHandler> promptHandlers) : IRequestHandler<AuthorizationCommand, Result>
{
    private readonly IClientManager _clientManager = clientManager;
    private readonly IEnumerable<IPromptHandler> _promptHandlers = promptHandlers;
    private readonly OpenIdConfiguration _configuration = options.Value;

    public async Task<Result> Handle(AuthorizationCommand request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(request.Request.ClientId))
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.InvalidRequest,
                Description = "client_id is required"
            });
        }

        var client = await _clientManager.FindByIdAsync(request.Request.ClientId, cancellationToken);
        if (client is null)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.InvalidRequest,
                Description = "Invalid client_id"
            });
        }

        string redirectUri;
        if (string.IsNullOrEmpty(request.Request.RedirectUri))
        {
            var redirectUris = client.Uris
                .Where(x => x.Type == UriType.Redirect)
                .ToList();

            if (redirectUris.Count != 1)
            {
                return Results.ClientError(ClientErrorCode.BadRequest, new Error()
                {
                    Code = ErrorCode.InvalidRequest,
                    Description = "redirect_uri is required"
                });
            }

            redirectUri = redirectUris.Single().Uri;
        }
        else
        {
            if (!client.HasUri(request.Request.RedirectUri, UriType.Redirect))
            {
                return Results.ClientError(ClientErrorCode.BadRequest, new Error()
                {
                    Code = ErrorCode.InvalidRequest,
                    Description = "Invalid redirect_uri"
                });
            }

            redirectUri = request.Request.RedirectUri;
        }

        if (string.IsNullOrEmpty(request.Request.ResponseType))
        {
            var uri = PromptHelper.GetRedirectUri(redirectUri, ErrorCode.UnsupportedResponseType,
                "response_type is required", request.Request.State);

            return Results.Redirect(RedirectionCode.Found, uri);
        }

        var responseType = EnumHelper.FromString<ResponseType>(request.Request.ResponseType);
        if (responseType is null)
        {
            var uri = PromptHelper.GetRedirectUri(redirectUri, ErrorCode.UnsupportedResponseType,
                "Invalid response_type", request.Request.State);

            return Results.Redirect(RedirectionCode.Found, uri);
        }

        if (!_configuration.ResponseTypesSupported.Contains(responseType.Value))
        {
            var uri = PromptHelper.GetRedirectUri(redirectUri, ErrorCode.UnsupportedResponseType,
                $"{responseType.Value.GetString()} is not supported", request.Request.State);

            return Results.Redirect(RedirectionCode.Found, uri);
        }

        if (client.ResponseTypes.All(x => x.ResponseType.Type != responseType.Value))
        {
            var uri = PromptHelper.GetRedirectUri(redirectUri, ErrorCode.UnsupportedResponseType,
                $"{responseType.Value.GetString()} is not supported by client", request.Request.State);

            return Results.Redirect(RedirectionCode.Found, uri);
        }

        if (string.IsNullOrEmpty(request.Request.Scope))
        {
            var uri = PromptHelper.GetRedirectUri(redirectUri, ErrorCode.InvalidScope,
                "scope is required", request.Request.State);

            return Results.Redirect(RedirectionCode.Found, uri);
        }

        var scopes = request.Request.Scope.Split(" ").ToList();
        var allowedScopes = client.AllowedScopes.Select(x => x.Scope.Value);
        if (!ScopesValidator.Validate(allowedScopes, scopes, out var invalidScopes))
        {
            var errorDescription = $"{string.Join(", ", invalidScopes)} are not supported scopes by client";
            var uri = PromptHelper.GetRedirectUri(redirectUri, ErrorCode.InvalidScope, errorDescription, request.Request.State);

            return Results.Redirect(RedirectionCode.Found, uri);
        }

        var prompts = new List<PromptType>();
        if (string.IsNullOrEmpty(request.Request.Prompt))
        {
            prompts.Add(PromptType.None);
        }
        else
        {
            var promptStrings = request.Request.Prompt.Split(" ").ToList();
            foreach (var promptString in promptStrings)
            {
                var prompt = EnumHelper.FromString<PromptType>(promptString);
                if (prompt is null)
                {
                    var uri = PromptHelper.GetRedirectUri(redirectUri, ErrorCode.InvalidRequest,
                        "Invalid prompt", request.Request.State);

                    return Results.Redirect(RedirectionCode.Found, uri);
                }

                if (!_configuration.PromptValuesSupported.Contains(prompt.Value))
                {
                    var uri = PromptHelper.GetRedirectUri(redirectUri, ErrorCode.InvalidRequest,
                        $"{prompt.Value.GetString()} is not supported prompt", request.Request.State);

                    return Results.Redirect(RedirectionCode.Found, uri);
                }

                prompts.Add(prompt.Value);
            }
        }

        ChallengeMethod? codeChallengeMethod = null;
        if (client.RequirePkce)
        {
            if (string.IsNullOrEmpty(request.Request.CodeChallenge))
            {
                var uri = PromptHelper.GetRedirectUri(redirectUri, ErrorCode.InvalidRequest,
                    "code_challenge is required", request.Request.State);

                return Results.Redirect(RedirectionCode.Found, uri);
            }

            if (string.IsNullOrEmpty(request.Request.CodeChallengeMethod))
            {
                var uri = PromptHelper.GetRedirectUri(redirectUri, ErrorCode.InvalidRequest,
                    "code_challenge_method is required", request.Request.State);

                return Results.Redirect(RedirectionCode.Found, uri);
            }

            var challengeMethod = EnumHelper.FromString<ChallengeMethod>(
                request.Request.CodeChallengeMethod);

            if (challengeMethod is null ||
                !_configuration.CodeChallengeMethodsSupported.Contains(challengeMethod.Value))
            {
                var uri = PromptHelper.GetRedirectUri(redirectUri, ErrorCode.InvalidRequest,
                    "Unsupported code_challenge_method", request.Request.State);

                return Results.Redirect(RedirectionCode.Found, uri);
            }

            codeChallengeMethod = challengeMethod.Value;
        }

        var processingPrompt = prompts.First();
        var promptContext = new PromptContext
        {
            ResponseType = responseType.Value,
            ClientId = client.Id,
            RedirectUri = redirectUri,
            Scopes = scopes,
            State = request.Request.State,
            Nonce = request.Request.Nonce,
            Prompts = prompts,
            CodeChallenge = request.Request.CodeChallenge,
            CodeChallengeMethod = codeChallengeMethod
        };

        while (true)
        {
            var handler = _promptHandlers.FirstOrDefault(h => h.CanHandle(processingPrompt));
            if (handler is null)
            {
                return Results.ServerError(ServerErrorCode.InternalServerError, new Error()
                {
                    Code = ErrorCode.ServerError,
                    Description = $"No handler for prompt {processingPrompt}"
                });
            }

            var promptResult = await handler.HandleAsync(promptContext, cancellationToken);
            if (promptResult.State is PromptState.Success or PromptState.Failed)
            {
                var result = promptResult.Result;
                if (result is null)
                {
                    return Results.ServerError(ServerErrorCode.InternalServerError, new Error()
                    {
                        Code = ErrorCode.ServerError,
                        Description = "Server error"
                    });
                }

                return result;
            }
            
            prompts.Remove(processingPrompt);
            if (prompts.Count == 0)
                prompts.Add(PromptType.None);

            processingPrompt = prompts.First();
        }
    }
}