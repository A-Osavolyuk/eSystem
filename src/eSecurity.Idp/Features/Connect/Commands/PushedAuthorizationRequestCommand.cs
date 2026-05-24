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

namespace eSecurity.Idp.Features.Connect.Commands;

public sealed record PushedAuthorizationRequestCommand(IFormCollection Form) : IRequest<Result>;

public sealed class PushedAuthorizationRequestCommandHandler(
    IFormBindingProvider bindingProvider,
    IParManager parManager,
    IClientManager clientManager,
    IOptions<OpenIdConfiguration> options) : IRequestHandler<PushedAuthorizationRequestCommand, Result>
{
    private readonly IFormBindingProvider _bindingProvider = bindingProvider;
    private readonly IParManager _parManager = parManager;
    private readonly IClientManager _clientManager = clientManager;
    private readonly OpenIdConfiguration _configuration = options.Value;

    public async Task<Result> Handle(PushedAuthorizationRequestCommand request, CancellationToken cancellationToken = default)
    {
        var binder = _bindingProvider.GetRequiredBinder<PushedAuthorizationRequest>();
        var bindingResult = await binder.BindAsync(request.Form, cancellationToken);
        if (!bindingResult.Succeeded)
        {
            var error = bindingResult.GetError();
            return Results.ClientError(ClientErrorCode.BadRequest, error);
        }

        if (!bindingResult.TryGetValue(out var par))
        {
            return Results.ServerError(ServerErrorCode.InternalServerError, new Error
            {
                Code = ErrorCode.ServerError,
                Description = "Server error"
            });
        }

        var client = await _clientManager.FindByIdAsync(par.ClientId, cancellationToken);
        if (client is null)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.InvalidRequest,
                Description = "Invalid client_id"
            });
        }
        
        string redirectUri;
        if (string.IsNullOrEmpty(par.RedirectUri))
        {
            var redirectUris = client.Uris
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
            if (!client.HasUri(par.RedirectUri, UriType.Redirect))
            {
                return Results.ClientError(ClientErrorCode.BadRequest, new Error
                {
                    Code = ErrorCode.InvalidRequest,
                    Description = "Invalid redirect_uri"
                });
            }

            redirectUri = par.RedirectUri;
        }

        if (!_configuration.ResponseTypesSupported.Contains(par.ResponseType))
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.UnsupportedResponseType,
                Description = $"{par.ResponseType.GetString()} is not supported"
            });
        }

        if (client.ResponseTypes.All(x => x.ResponseType.Type != par.ResponseType))
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.UnsupportedResponseType,
                Description = $"{par.ResponseType.GetString()} is not supported by client"
            });
        }

        var scopes = par.Scope.Split(" ").ToList();
        var allowedScopes = client.AllowedScopes.Select(x => x.Scope.Value);
        if (!ScopesValidator.Validate(allowedScopes, scopes, out var invalidScopes))
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.InvalidUserCode,
                Description = $"{string.Join(", ", invalidScopes)} are not supported scopes by client"
            });
        }

        var prompts = new List<PromptType>();
        if (string.IsNullOrEmpty(par.Prompt))
        {
            prompts.Add(PromptType.None);
        }
        else
        {
            var promptStrings = par.Prompt.Split(" ").ToList();
            foreach (var promptString in promptStrings)
            {
                var prompt = EnumHelper.FromString<PromptType>(promptString);
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
        
        var hasCodeChallenge = !string.IsNullOrEmpty(par.CodeChallenge);
        var hasCodeChallengeMethod = par.CodeChallengeMethod is not null;
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
            if (!_configuration.CodeChallengeMethodsSupported.Contains(par.CodeChallengeMethod!.Value))
            {
                return Results.ClientError(ClientErrorCode.BadRequest, new Error
                {
                    Code = ErrorCode.InvalidRequest,
                    Description = "Unsupported code_challenge_method"
                });
            }

            codeChallengeMethod = par.CodeChallengeMethod.Value;
        }

        //TODO: Add PAR configurations
        var parEntity = new PushedAuthorizationRequestEntity
        {
            Id = Guid.CreateVersion7(),
            RequestUri = ParHelper.GetRequestUri(),
            ResponseType = par.ResponseType,
            ClientId = client.Id,
            RedirectUri = redirectUri,
            Nonce = par.Nonce,
            State = par.State,
            CodeChallenge = par.CodeChallenge,
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