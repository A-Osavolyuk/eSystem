using eSecurity.Core.Common.Requests;
using eSecurity.Core.Common.Responses;
using eSecurity.Server.Data.Entities;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Client;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Session;
using eSecurity.Server.Security.Authorization.OAuth.Consents;
using eSecurity.Server.Security.Authorization.OAuth.Protocol;
using eSecurity.Server.Security.Authorization.OAuth.Token.AuthorizationCode;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Mediator;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;
using eSystem.Core.Security.Authentication.OpenIdConnect;
using eSystem.Core.Security.Authentication.OpenIdConnect.Client;
using eSystem.Core.Security.Authentication.OpenIdConnect.Discovery;

namespace eSecurity.Server.Features.Connect.Commands;

public record AuthorizeCommand(AuthorizeRequest Request) : IRequest<Result>;

public class AuthorizeCommandHandler(
    IUserManager userManager,
    ISessionManager sessionManager,
    IAuthorizationCodeManager authorizationCodeManager,
    IClientManager clientManager,
    IConsentManager consentManager,
    IOptions<OpenIdConfiguration> options) : IRequestHandler<AuthorizeCommand, Result>
{
    private readonly IUserManager _userManager = userManager;
    private readonly ISessionManager _sessionManager = sessionManager;
    private readonly IAuthorizationCodeManager _authorizationCodeManager = authorizationCodeManager;
    private readonly IClientManager _clientManager = clientManager;
    private readonly IConsentManager _consentManager = consentManager;
    private readonly OpenIdConfiguration _configuration = options.Value;

    public async Task<Result> Handle(AuthorizeCommand request, CancellationToken cancellationToken)
    {
        if (!_configuration.ResponseTypesSupported.Contains(request.Request.ResponseType))
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.UnsupportedResponseType,
                Description = $"'{request.Request.ResponseType}' is unsupported response type"
            });
        }

        if (string.IsNullOrWhiteSpace(request.Request.Nonce))
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.InvalidRequest,
                Description = "nonce is required."
            });
        }
        
        var unsupportedScopes = request.Request.Scopes
            .Where(x => !_configuration.ScopesSupported.Contains(x))
            .ToList();

        if (unsupportedScopes.Count > 0)
        {
            if (unsupportedScopes.Count == 1)
            {
                return Results.ClientError(ClientErrorCode.BadRequest, new Error
                {
                    Code = ErrorCode.InvalidScope,
                    Description = $"'{unsupportedScopes.First()}' scope is invalid."
                });
            }

            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.InvalidScope,
                Description = $"'{string.Join(" ", unsupportedScopes)}' scopes are invalid."
            });
        }
        
        var client = await _clientManager.FindByIdAsync(request.Request.ClientId, cancellationToken);
        if (client is null)
        {
            return Results.ClientError(ClientErrorCode.Unauthorized, new Error
            {
                Code = ErrorCode.InvalidClient,
                Description = "Invalid client"
            });
        }

        if (!client.HasUri(request.Request.RedirectUri, UriType.Redirect))
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.InvalidRequest,
                Description = "redirect_uri is invalid."
            });
        }
        
        if (!client.HasScopes(request.Request.Scopes, out var scopes))
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.InvalidScope,
                Description = $"'{string.Join(',', scopes)}' are not supported scopes."
            });
        }

        if (client is { ClientType: ClientType.Public, RequirePkce: true })
        {
            if (string.IsNullOrEmpty(request.Request.CodeChallenge))
            {
                return Results.ClientError(ClientErrorCode.BadRequest, new Error
                {
                    Code = ErrorCode.InvalidRequest,
                    Description = "code_challenge is required"
                });
            }

            if (!request.Request.CodeChallengeMethod.HasValue)
            {
                return Results.ClientError(ClientErrorCode.BadRequest, new Error
                {
                    Code = ErrorCode.InvalidRequest,
                    Description = "code_challenge_method is required"
                });
            }
        }
        
        var session = await _sessionManager.FindByIdAsync(request.Request.SessionId, cancellationToken);
        if (session is null)
        {
            return Results.ServerError(ServerErrorCode.InternalServerError, new Error
            {
                Code = ErrorCode.ServerError,
                Description = "Invalid authorization session."
            });
        }

        var user = await _userManager.FindByIdAsync(session.UserId, cancellationToken);
        if (user is null)
        {
            return Results.ServerError(ServerErrorCode.InternalServerError, new Error
            {
                Code = ErrorCode.ServerError,
                Description = "Invalid authorization session."
            });
        }

        var consent = await _consentManager.FindAsync(user, client, cancellationToken);
        if (consent is null || !consent.HasScopes(request.Request.Scopes, out _))
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.ConsentRequired,
                Description = "User consent is required."
            });
        }

        var protocol = request.Request.Scopes.Contains(ScopeTypes.OpenId)
            ? AuthorizationProtocol.OpenIdConnect
            : AuthorizationProtocol.OAuth;
        
        var code = _authorizationCodeManager.Generate();
        var authorizationCode = new AuthorizationCodeEntity
        {
            Id = Guid.CreateVersion7(),
            ClientId = client.Id,
            UserId = user.Id,
            Protocol = protocol,
            Code = code,
            Nonce = request.Request.Nonce,
            CodeChallenge = request.Request.CodeChallenge,
            CodeChallengeMethod = request.Request.CodeChallengeMethod,
            RedirectUri = request.Request.RedirectUri,
            ExpiredAt = DateTimeOffset.UtcNow.AddMinutes(10),
        };

        var codeResult = await _authorizationCodeManager.CreateAsync(authorizationCode, cancellationToken);
        if (!codeResult.Succeeded) return codeResult;

        var response = new AuthorizeResponse
        {
            State = request.Request.State,
            Code = code
        };

        return Results.Success(SuccessCodes.Ok, response);
    }
}