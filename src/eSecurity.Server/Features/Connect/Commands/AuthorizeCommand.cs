using eSecurity.Core.Common.Requests;
using eSecurity.Core.Common.Responses;
using eSecurity.Core.Security.Authentication.Oidc;
using eSecurity.Server.Data.Entities;
using eSecurity.Server.Security.Authentication.Oidc.Client;
using eSecurity.Server.Security.Authentication.Oidc.Code;
using eSecurity.Server.Security.Authentication.Oidc.Session;
using eSecurity.Server.Security.Authorization.Consents;
using eSecurity.Server.Security.Authorization.Devices;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Common.Http.Context;

namespace eSecurity.Server.Features.Connect.Commands;

public record AuthorizeCommand(AuthorizeRequest Request) : IRequest<Result>;

public class AuthorizeCommandHandler(
    IUserManager userManager,
    IHttpContextAccessor httpContextAccessor,
    ISessionManager sessionManager,
    IAuthorizationCodeManager authorizationCodeManager,
    IClientManager clientManager,
    IConsentManager consentManager,
    IDeviceManager deviceManager,
    IOptions<OpenIdOptions> options) : IRequestHandler<AuthorizeCommand, Result>
{
    private readonly IUserManager _userManager = userManager;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly ISessionManager _sessionManager = sessionManager;
    private readonly IAuthorizationCodeManager _authorizationCodeManager = authorizationCodeManager;
    private readonly IClientManager _clientManager = clientManager;
    private readonly IConsentManager _consentManager = consentManager;
    private readonly IDeviceManager _deviceManager = deviceManager;
    private readonly OpenIdOptions _options = options.Value;

    public async Task<Result> Handle(AuthorizeCommand request, CancellationToken cancellationToken)
    {
        if (!_options.ResponseTypesSupported.Contains(request.Request.ResponseType))
        {
            return Results.BadRequest(new Error()
            {
                Code = Errors.OAuth.UnsupportedResponseType,
                Description = $"'{request.Request.ResponseType}' is unsupported response type"
            });
        }

        if (string.IsNullOrWhiteSpace(request.Request.Nonce))
        {
            return Results.BadRequest(new Error()
            {
                Code = Errors.OAuth.InvalidRequest,
                Description = "nonce is required."
            });
        }
        
        var unsupportedScopes = request.Request.Scopes
            .Where(x => !_options.ScopesSupported.Contains(x))
            .ToList();

        if (unsupportedScopes.Count > 0)
        {
            if (unsupportedScopes.Count == 1)
            {
                return Results.BadRequest(new Error()
                {
                    Code = Errors.OAuth.InvalidScope,
                    Description = $"'{unsupportedScopes.First()}' scope is invalid."
                });
            }

            return Results.BadRequest(new Error()
            {
                Code = Errors.OAuth.InvalidScope,
                Description = $"'{string.Join(" ", unsupportedScopes)}' scopes are invalid."
            });
        }
        
        var client = await _clientManager.FindByIdAsync(request.Request.ClientId, cancellationToken);
        if (client is null)
        {
            return Results.Unauthorized(new Error()
            {
                Code = Errors.OAuth.InvalidClient,
                Description = "Invalid client"
            });
        }

        if (!client.HasUri(request.Request.RedirectUri, UriType.Redirect))
        {
            return Results.BadRequest(new Error()
            {
                Code = Errors.OAuth.InvalidRequest,
                Description = "redirect_uri is invalid."
            });
        }
        
        if (!client.HasScopes(request.Request.Scopes))
        {
            return Results.BadRequest(new Error()
            {
                Code = Errors.OAuth.InvalidScope,
                Description = "Invalid scopes."
            });
        }

        if (client is { ClientType: ClientType.Public, RequirePkce: true })
        {
            if (string.IsNullOrEmpty(request.Request.CodeChallenge))
                return Results.BadRequest(new Error()
                {
                    Code = Errors.OAuth.InvalidRequest,
                    Description = "code_challenge is required"
                });

            if (string.IsNullOrEmpty(request.Request.CodeChallengeMethod))
                return Results.BadRequest(new Error()
                {
                    Code = Errors.OAuth.InvalidRequest,
                    Description = "code_challenge_method is required"
                });
        }

        var user = await _userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null)
        {
            return Results.InternalServerError(new Error()
            {
                Code = Errors.OAuth.ServerError,
                Description = "Invalid authorization session."
            });
        }

        var consent = await _consentManager.FindAsync(user, client, cancellationToken);
        if (consent is null || !consent.HasScopes(request.Request.Scopes, out _))
        {
            return Results.BadRequest(new Error()
            {
                Code = Errors.OAuth.ConsentRequired,
                Description = "User consent is required."
            });
        }

        var userAgent = _httpContextAccessor.HttpContext?.GetUserAgent()!;
        var ipAddress = _httpContextAccessor.HttpContext?.GetIpV4()!;

        var device = await _deviceManager.FindAsync(user, userAgent, ipAddress, cancellationToken);
        if (device is null)
        {
            return Results.InternalServerError(new Error()
            {
                Code = Errors.OAuth.ServerError,
                Description = "Invalid authorization session."
            });
        }

        var session = await _sessionManager.FindAsync(device, cancellationToken);
        if (session is null)
        {
            return Results.InternalServerError(new Error()
            {
                Code = Errors.OAuth.ServerError,
                Description = "Invalid authorization session."
            });
        }

        var code = _authorizationCodeManager.Generate();
        var authorizationCode = new AuthorizationCodeEntity()
        {
            Id = Guid.CreateVersion7(),
            ClientId = client.Id,
            DeviceId = device.Id,
            Code = code,
            Nonce = request.Request.Nonce,
            CodeChallenge = request.Request.CodeChallenge,
            CodeChallengeMethod = request.Request.CodeChallengeMethod,
            RedirectUri = request.Request.RedirectUri,
            ExpireDate = DateTimeOffset.UtcNow.AddMinutes(10),
        };

        var codeResult = await _authorizationCodeManager.CreateAsync(authorizationCode, cancellationToken);
        if (!codeResult.Succeeded) return codeResult;

        var response = new AuthorizeResponse()
        {
            UserId = user.Id,
            SessionId = session.Id,
            DeviceId = device.Id,
            State = request.Request.State,
            Code = code
        };

        return Results.Ok(response);
    }
}