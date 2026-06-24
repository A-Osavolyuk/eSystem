using eSecurity.Idp.Features.Connect;
using eSecurity.Idp.Security.Authentication.Client;
using eSecurity.Idp.Security.Authorization.Prompt;
using eSecurity.Idp.Security.Authorization.Scopes;
using eSystem.Core.Enums;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;
using eSystem.Core.Security.Authentication.OpenIdConnect;
using eSystem.Core.Server.Security.Authentication.OpenIdConnect.Discovery;

namespace eSecurity.Idp.Security.Authorization.Authorize.Manual;

public sealed class ManualAuthorizationFlowHandler(
    IOptions<OpenIdConfiguration> options,
    IPromptsProcessor promptsProcessor,
    IClientQueryService clientQueryService,
    RedirectManager redirectManager) : IAuthorizationFlowHandler
{
    private readonly OpenIdConfiguration _configuration = options.Value;
    private readonly IPromptsProcessor _promptsProcessor = promptsProcessor;
    private readonly IClientQueryService _clientQueryService = clientQueryService;
    private readonly RedirectManager _redirectManager = redirectManager;

    public async ValueTask<Result> HandleAsync(AuthorizationCommand command,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(command.ClientId))
        {
            var uri = _redirectManager.GetRedirectUri(ErrorCode.InvalidRequest, 
                "client_id is required");
            
            return Results.Redirect(RedirectionCode.Found, uri);
        }

        var client = await _clientQueryService.GetByIdAsync(command.ClientId, cancellationToken);
        if (client is null)
        {
            var uri = _redirectManager.GetRedirectUri(ErrorCode.InvalidRequest, 
                "client_id is invalid");
            
            return Results.Redirect(RedirectionCode.Found, uri);
        }

        var clientUris = await _clientQueryService.GetUrisAsync(client, cancellationToken);
        
        string redirectUri;
        if (string.IsNullOrEmpty(command.RedirectUri))
        {
            var redirectUris = clientUris
                .Where(x => x.Type == UriType.Redirect)
                .ToList();

            if (redirectUris.Count != 1)
            {
                var uri = _redirectManager.GetRedirectUri(ErrorCode.InvalidRequest, 
                    "redirect_uri is required");
            
                return Results.Redirect(RedirectionCode.Found, uri);
            }

            redirectUri = redirectUris.Single().Uri;
        }
        else
        {
            if (clientUris.All(x => x.Type != UriType.Redirect && x.Uri != command.RedirectUri))
            {
                var uri = _redirectManager.GetRedirectUri(ErrorCode.InvalidRequest, 
                    "redirect_uri is invalid");
            
                return Results.Redirect(RedirectionCode.Found, uri);
            }

            redirectUri = command.RedirectUri;
        }

        if (string.IsNullOrEmpty(command.ResponseType))
        {
            var uri = _redirectManager.GetRedirectUri(redirectUri, ErrorCode.UnsupportedResponseType, 
                "response_type is required", command.State);
            
            return Results.Redirect(RedirectionCode.Found, uri);
        }

        var responseType = EnumHelper.ParseFromString<ResponseType>(command.ResponseType);
        if (responseType is null)
        {
            var uri = _redirectManager.GetRedirectUri(redirectUri, ErrorCode.UnsupportedResponseType, 
                "response_type is invalid", command.State);
            
            return Results.Redirect(RedirectionCode.Found, uri);
        }

        if (!_configuration.ResponseTypesSupported.Contains(responseType.Value))
        {
            var uri = _redirectManager.GetRedirectUri(redirectUri, ErrorCode.UnsupportedResponseType, 
                $"{responseType.Value.GetString()} is not supported", command.State);
            
            return Results.Redirect(RedirectionCode.Found, uri);
        }

        var clientResponseTypes = await _clientQueryService.GetSupportedResponseTypesAsync(
            client, cancellationToken);
        
        if (clientResponseTypes.All(x => x.ResponseType.Type != responseType.Value))
        {
            var uri = _redirectManager.GetRedirectUri(redirectUri, ErrorCode.UnsupportedResponseType, 
                $"{responseType.Value.GetString()} is not supported by client", command.State);
            
            return Results.Redirect(RedirectionCode.Found, uri);
        }

        if (string.IsNullOrEmpty(command.Scope))
        {
            var uri = _redirectManager.GetRedirectUri(redirectUri, ErrorCode.InvalidScope, 
                "scope is required", command.State);
            
            return Results.Redirect(RedirectionCode.Found, uri);
        }

        var clientScopes = await _clientQueryService.GetAllowedScopesAsync(
            client, cancellationToken);
        
        var scopes = command.Scope.Split(" ").ToList();
        var allowedScopes = clientScopes.Select(x => x.Scope.Value);
        if (!ScopesValidator.Validate(allowedScopes, scopes, out var invalidScopes))
        {
            var errorDescription = $"{string.Join(", ", invalidScopes)} are not supported scopes by client";
            var uri = _redirectManager.GetRedirectUri(redirectUri, ErrorCode.InvalidScope, 
                errorDescription, command.State);
            
            return Results.Redirect(RedirectionCode.Found, uri);
        }

        var prompts = new List<PromptType>();
        if (string.IsNullOrEmpty(command.Prompt))
        {
            prompts.Add(PromptType.None);
        }
        else
        {
            var promptStrings = command.Prompt.Split(" ").ToList();
            foreach (var promptString in promptStrings)
            {
                var prompt = EnumHelper.ParseFromString<PromptType>(promptString);
                if (prompt is null)
                {
                    var uri = _redirectManager.GetRedirectUri(redirectUri, ErrorCode.InvalidRequest, 
                        "prompt is invalid", command.State);
            
                    return Results.Redirect(RedirectionCode.Found, uri);
                }

                if (!_configuration.PromptValuesSupported.Contains(prompt.Value))
                {
                    var uri = _redirectManager.GetRedirectUri(redirectUri, ErrorCode.InvalidRequest, 
                        $"{prompt.Value.GetString()} is not supported prompt", command.State);
            
                    return Results.Redirect(RedirectionCode.Found, uri);
                }

                prompts.Add(prompt.Value);
            }
        }

        var hasCodeChallenge = !string.IsNullOrEmpty(command.CodeChallenge);
        var hasCodeChallengeMethod = !string.IsNullOrEmpty(command.CodeChallengeMethod);
        if (client.RequirePkce)
        {
            if (!hasCodeChallenge)
            {
                var uri = _redirectManager.GetRedirectUri(redirectUri, ErrorCode.InvalidRequest, 
                    "code_challenge is required", command.State);
            
                return Results.Redirect(RedirectionCode.Found, uri);
            }

            if (!hasCodeChallengeMethod)
            {
                var uri = _redirectManager.GetRedirectUri(redirectUri, ErrorCode.InvalidRequest, 
                    "code_challenge_method is required", command.State);
            
                return Results.Redirect(RedirectionCode.Found, uri);
            }
        }
        else
        {
            if (hasCodeChallenge ^ hasCodeChallengeMethod)
            {
                var uri = _redirectManager.GetRedirectUri(redirectUri, ErrorCode.InvalidRequest, 
                    "code_challenge and code_challenge_method must be provided together", command.State);
            
                return Results.Redirect(RedirectionCode.Found, uri);
            }
        }

        ChallengeMethod? codeChallengeMethod = null;
        if (hasCodeChallenge && hasCodeChallengeMethod)
        {
            var methodString = command.CodeChallengeMethod;
            if (string.IsNullOrWhiteSpace(methodString) ||
                EnumHelper.TryParseFromString<ChallengeMethod>(methodString, out var challengeMethod))
            {
                var uri = _redirectManager.GetRedirectUri(redirectUri, ErrorCode.InvalidRequest, 
                    "code_challenge_method is invalid", command.State);
            
                return Results.Redirect(RedirectionCode.Found, uri);
            }

            if (!_configuration.CodeChallengeMethodsSupported.Contains(challengeMethod.Value))
            {
                var uri = _redirectManager.GetRedirectUri(redirectUri, ErrorCode.InvalidRequest, 
                    "code_challenge_method is unsupported", command.State);
            
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
            State = command.State,
            Nonce = command.Nonce,
            Prompts = prompts,
            CodeChallenge = command.CodeChallenge,
            CodeChallengeMethod = codeChallengeMethod
        };

        return await _promptsProcessor.ProcessAsync(promptContext, cancellationToken);
    }
}