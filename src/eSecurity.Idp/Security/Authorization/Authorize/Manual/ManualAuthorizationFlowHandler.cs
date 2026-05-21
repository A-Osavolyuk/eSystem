using eSecurity.Idp.Security.Authentication.OpenIdConnect.Client;
using eSecurity.Idp.Security.Authentication.OpenIdConnect.Prompt;
using eSecurity.Idp.Security.Authorization.OAuth.Scopes;
using eSystem.Core.Enums;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;
using eSystem.Core.Security.Authentication.OpenIdConnect;
using eSystem.Core.Server.Security.Authentication.OpenIdConnect.Discovery;

namespace eSecurity.Idp.Security.Authorization.Authorize.Manual;

public sealed class ManualAuthorizationFlowHandler(
    IClientManager clientManager,
    IOptions<OpenIdConfiguration> options,
    IPromptsProcessor promptsProcessor) : IAuthorizationFlowHandler
{
    private readonly IClientManager _clientManager = clientManager;
    private readonly OpenIdConfiguration _configuration = options.Value;
    private readonly IPromptsProcessor _promptsProcessor = promptsProcessor;

    public async ValueTask<Result> HandleAsync(AuthorizationRequest request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(request.ClientId))
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.InvalidRequest,
                Description = "client_id is required"
            });
        }

        var client = await _clientManager.FindByIdAsync(request.ClientId, cancellationToken);
        if (client is null)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.InvalidRequest,
                Description = "Invalid client_id"
            });
        }

        string redirectUri;
        if (string.IsNullOrEmpty(request.RedirectUri))
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
            if (!client.HasUri(request.RedirectUri, UriType.Redirect))
            {
                return Results.ClientError(ClientErrorCode.BadRequest, new Error()
                {
                    Code = ErrorCode.InvalidRequest,
                    Description = "Invalid redirect_uri"
                });
            }

            redirectUri = request.RedirectUri;
        }

        if (string.IsNullOrEmpty(request.ResponseType))
        {
            var uri = PromptHelper.GetRedirectUri(redirectUri, ErrorCode.UnsupportedResponseType,
                "response_type is required", request.State);

            return Results.Redirect(RedirectionCode.Found, uri);
        }

        var responseType = EnumHelper.FromString<ResponseType>(request.ResponseType);
        if (responseType is null)
        {
            var uri = PromptHelper.GetRedirectUri(redirectUri, ErrorCode.UnsupportedResponseType,
                "Invalid response_type", request.State);

            return Results.Redirect(RedirectionCode.Found, uri);
        }

        if (!_configuration.ResponseTypesSupported.Contains(responseType.Value))
        {
            var uri = PromptHelper.GetRedirectUri(redirectUri, ErrorCode.UnsupportedResponseType,
                $"{responseType.Value.GetString()} is not supported", request.State);

            return Results.Redirect(RedirectionCode.Found, uri);
        }

        if (client.ResponseTypes.All(x => x.ResponseType.Type != responseType.Value))
        {
            var uri = PromptHelper.GetRedirectUri(redirectUri, ErrorCode.UnsupportedResponseType,
                $"{responseType.Value.GetString()} is not supported by client", request.State);

            return Results.Redirect(RedirectionCode.Found, uri);
        }

        if (string.IsNullOrEmpty(request.Scope))
        {
            var uri = PromptHelper.GetRedirectUri(redirectUri, ErrorCode.InvalidScope,
                "scope is required", request.State);

            return Results.Redirect(RedirectionCode.Found, uri);
        }

        var scopes = request.Scope.Split(" ").ToList();
        var allowedScopes = client.AllowedScopes.Select(x => x.Scope.Value);
        if (!ScopesValidator.Validate(allowedScopes, scopes, out var invalidScopes))
        {
            var errorDescription = $"{string.Join(", ", invalidScopes)} are not supported scopes by client";
            var uri = PromptHelper.GetRedirectUri(redirectUri, ErrorCode.InvalidScope, errorDescription,
                request.State);

            return Results.Redirect(RedirectionCode.Found, uri);
        }

        var prompts = new List<PromptType>();
        if (string.IsNullOrEmpty(request.Prompt))
        {
            prompts.Add(PromptType.None);
        }
        else
        {
            var promptStrings = request.Prompt.Split(" ").ToList();
            foreach (var promptString in promptStrings)
            {
                var prompt = EnumHelper.FromString<PromptType>(promptString);
                if (prompt is null)
                {
                    var uri = PromptHelper.GetRedirectUri(redirectUri, ErrorCode.InvalidRequest,
                        "Invalid prompt", request.State);

                    return Results.Redirect(RedirectionCode.Found, uri);
                }

                if (!_configuration.PromptValuesSupported.Contains(prompt.Value))
                {
                    var uri = PromptHelper.GetRedirectUri(redirectUri, ErrorCode.InvalidRequest,
                        $"{prompt.Value.GetString()} is not supported prompt", request.State);

                    return Results.Redirect(RedirectionCode.Found, uri);
                }

                prompts.Add(prompt.Value);
            }
        }

        var hasCodeChallenge = !string.IsNullOrEmpty(request.CodeChallenge);
        var hasCodeChallengeMethod = !string.IsNullOrEmpty(request.CodeChallengeMethod);
        if (client.RequirePkce)
        {
            if (!hasCodeChallenge)
            {
                var uri = PromptHelper.GetRedirectUri(redirectUri, ErrorCode.InvalidRequest,
                    "code_challenge is required", request.State);

                return Results.Redirect(RedirectionCode.Found, uri);
            }

            if (!hasCodeChallengeMethod)
            {
                var uri = PromptHelper.GetRedirectUri(redirectUri, ErrorCode.InvalidRequest,
                    "code_challenge_method is required", request.State);

                return Results.Redirect(RedirectionCode.Found, uri);
            }
        }
        else
        {
            if (hasCodeChallenge ^ hasCodeChallengeMethod)
            {
                var uri = PromptHelper.GetRedirectUri(redirectUri, ErrorCode.InvalidRequest,
                    "code_challenge and code_challenge_method must be provided together",
                    request.State);

                return Results.Redirect(RedirectionCode.Found, uri);
            }
        }

        ChallengeMethod? codeChallengeMethod = null;
        if (hasCodeChallenge && hasCodeChallengeMethod)
        {
            var challengeMethod = EnumHelper.FromString<ChallengeMethod>(
                request.CodeChallengeMethod
            );

            if (challengeMethod is null)
            {
                var uri = PromptHelper.GetRedirectUri(redirectUri, ErrorCode.InvalidRequest,
                    "code_challenge_method is invalid", request.State);

                return Results.Redirect(RedirectionCode.Found, uri);
            }

            if (!_configuration.CodeChallengeMethodsSupported.Contains(challengeMethod.Value))
            {
                var uri = PromptHelper.GetRedirectUri(redirectUri, ErrorCode.InvalidRequest,
                    "Unsupported code_challenge_method", request.State);

                return Results.Redirect(RedirectionCode.Found, uri);
            }

            codeChallengeMethod = challengeMethod.Value;
        }
        
        var promptContext = new PromptContext
        {
            AuthorizationFlow = AuthorizationFlow.Manual,
            ResponseType = responseType.Value,
            ClientId = client.Id,
            RedirectUri = redirectUri,
            Scopes = scopes,
            State = request.State,
            Nonce = request.Nonce,
            Prompts = prompts,
            CodeChallenge = request.CodeChallenge,
            CodeChallengeMethod = codeChallengeMethod
        };

        return await _promptsProcessor.ProcessAsync(promptContext, cancellationToken);
    }
}