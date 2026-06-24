using eSecurity.Idp.Data.Entities;
using eSecurity.Idp.Security.Authentication.Client;
using eSecurity.Idp.Security.Authorization.Authorize.Par;
using eSecurity.Idp.Security.Authorization.Scopes;
using eSystem.Core.Enums;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;
using eSystem.Core.Security.Authentication.OpenIdConnect;
using eSystem.Core.Server.Binding;
using eSystem.Core.Server.Security.Authentication.OpenIdConnect.Discovery;
using eSystem.Core.Server.Security.Authorization.OAuth.Par;

namespace eSecurity.Idp.Features.Connect;

public sealed record PushedAuthorizationRequestCommand : IRequest<Result>
{
    [FromForm(Name = "response_type")]
    public ResponseType ResponseType { get; init; }
      
    [FromForm(Name = "client_id")]
    public string ClientId { get; init; } = null!;
    
    [FromForm(Name = "scope")]
    public string Scope { get; init; } = null!;
    
    [FromForm(Name = "redirect_uri")]
    public string? RedirectUri { get; init; }
    
    [FromForm(Name = "nonce")]
    public string? Nonce { get; init; }
    
    [FromForm(Name = "state")]
    public string? State { get; init; }
    
    [FromForm(Name = "prompt")]
    public string? Prompt { get; init; }
    
    [FromForm(Name = "code_challenge")]
    public string? CodeChallenge { get; init; }
    
    [FromForm(Name = "code_challenge_method")]
    public ChallengeMethod? CodeChallengeMethod { get; init; }
}

public sealed class PushedAuthorizationRequestCommandHandler(
    IParManager parManager,
    IClientQueryService clientQueryService,
    IOptions<OpenIdConfiguration> options) : IRequestHandler<PushedAuthorizationRequestCommand, Result>
{
    private readonly IParManager _parManager = parManager;
    private readonly IClientQueryService _clientQueryService = clientQueryService;
    private readonly OpenIdConfiguration _configuration = options.Value;

    public async Task<Result> Handle(PushedAuthorizationRequestCommand request, CancellationToken cancellationToken = default)
    {
        var client = await _clientQueryService.GetByIdAsync(request.ClientId, cancellationToken);
        if (client is null)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.InvalidRequest,
                Description = "Invalid client_id"
            });
        }

        var clientUris = await _clientQueryService.GetUrisAsync(client, cancellationToken);
;        
        string redirectUri;
        if (string.IsNullOrEmpty(request.RedirectUri))
        {
            var redirectUris = clientUris
                .Where(x => x.Type == UriType.Redirect)
                .ToList();

            if (redirectUris.Count != 1)
            {
                return Results.ClientError(ClientErrorCode.BadRequest, new Error
                {
                    Code = ErrorCode.InvalidRequest,
                    Description = "redirect_uri is required"
                });
            }

            redirectUri = redirectUris.Single().Uri;
        }
        else
        {
            if (clientUris.All(x => x.Type != UriType.Redirect && x.Uri != request.RedirectUri))
            {
                return Results.ClientError(ClientErrorCode.BadRequest, new Error
                {
                    Code = ErrorCode.InvalidRequest,
                    Description = "Invalid redirect_uri"
                });
            }

            redirectUri = request.RedirectUri;
        }

        if (!_configuration.ResponseTypesSupported.Contains(request.ResponseType))
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.UnsupportedResponseType,
                Description = $"{request.ResponseType.GetString()} is not supported"
            });
        }

        var clientResponseTypes = await _clientQueryService.GetSupportedResponseTypesAsync(
            client, cancellationToken);
        
        if (clientResponseTypes.All(x => x.ResponseType.Type != request.ResponseType))
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.UnsupportedResponseType,
                Description = $"{request.ResponseType.GetString()} is not supported by client"
            });
        }

        var clientScopes = await _clientQueryService.GetAllowedScopesAsync(
            client, cancellationToken);
        
        var scopes = request.Scope.Split(" ").ToList();
        var allowedScopes = clientScopes.Select(x => x.Scope.Value);
        if (!ScopesValidator.Validate(allowedScopes, scopes, out var invalidScopes))
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.InvalidUserCode,
                Description = $"{string.Join(", ", invalidScopes)} are not supported scopes by client"
            });
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
                var prompt = EnumHelper.ParseFromString<PromptType>(promptString);
                if (prompt is null)
                {
                    return Results.ClientError(ClientErrorCode.BadRequest, new Error
                    {
                        Code = ErrorCode.InvalidRequest,
                        Description = "Invalid prompt"
                    });
                }

                if (!_configuration.PromptValuesSupported.Contains(prompt.Value))
                {
                    return Results.ClientError(ClientErrorCode.BadRequest, new Error
                    {
                        Code = ErrorCode.InvalidRequest,
                        Description = $"{prompt.Value.GetString()} is not supported prompt"
                    });
                }

                prompts.Add(prompt.Value);
            }
        }
        
        var hasCodeChallenge = !string.IsNullOrEmpty(request.CodeChallenge);
        var hasCodeChallengeMethod = request.CodeChallengeMethod is not null;
        if (client.RequirePkce)
        {
            if (!hasCodeChallenge)
            {
                return Results.ClientError(ClientErrorCode.BadRequest, new Error
                {
                    Code = ErrorCode.InvalidRequest,
                    Description = "code_challenge is required"
                });
            }

            if (!hasCodeChallengeMethod)
            {
                return Results.ClientError(ClientErrorCode.BadRequest, new Error
                {
                    Code = ErrorCode.InvalidRequest,
                    Description = "code_challenge_method is required"
                });
            }
        }
        else
        {
            if (hasCodeChallenge ^ hasCodeChallengeMethod)
            {
                return Results.ClientError(ClientErrorCode.BadRequest, new Error
                {
                    Code = ErrorCode.InvalidRequest,
                    Description = "code_challenge and code_challenge_method must be provided together"
                });
            }
        }

        ChallengeMethod? codeChallengeMethod = null;
        if (hasCodeChallenge && hasCodeChallengeMethod)
        {
            if (!_configuration.CodeChallengeMethodsSupported.Contains(request.CodeChallengeMethod!.Value))
            {
                return Results.ClientError(ClientErrorCode.BadRequest, new Error
                {
                    Code = ErrorCode.InvalidRequest,
                    Description = "Unsupported code_challenge_method"
                });
            }

            codeChallengeMethod = request.CodeChallengeMethod.Value;
        }

        //TODO: Add PAR configurations
        var parEntity = new PushedAuthorizationRequestEntity
        {
            Id = Guid.CreateVersion7(),
            RequestUri = ParHelper.GetRequestUri(),
            ResponseType = request.ResponseType,
            ClientId = client.Id,
            RedirectUri = redirectUri,
            Nonce = request.Nonce,
            State = request.State,
            CodeChallenge = request.CodeChallenge,
            CodeChallengeMethod = codeChallengeMethod,
            Status = ParState.Pending,
            ExpiredAt = DateTimeOffset.UtcNow.AddSeconds(120)
        };

        parEntity.AddPrompts(prompts);
        parEntity.AddScopes(scopes);

        var result = await _parManager.CreateAsync(parEntity, cancellationToken);
        if (!result.Succeeded)
            return result;

        var response = new PushedAuthorizationResponse
        {
            ClientId = parEntity.ClientId.ToString(),
            RequestUri = parEntity.RequestUri
        };

        return Results.Success(SuccessCodes.Ok, response);
    }
}