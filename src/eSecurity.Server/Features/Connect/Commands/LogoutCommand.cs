using eSecurity.Core.Common.Requests;
using eSecurity.Core.Common.Responses;
using eSecurity.Server.Data.Entities;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Client;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Constants;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Session;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Token.Validation;
using eSecurity.Server.Security.Cryptography.Tokens;
using eSystem.Core.Http.Constants;
using eSystem.Core.Http.Results;
using eSystem.Core.Security.Identity.Claims;

namespace eSecurity.Server.Features.Connect.Commands;

public record LogoutCommand(LogoutRequest Request) : IRequest<Result>;

public class LogoutCommandHandler(
    IClientManager clientManager,
    ISessionManager sessionManager,
    ITokenValidationProvider validationProvider) : IRequestHandler<LogoutCommand, Result>
{
    private readonly IClientManager _clientManager = clientManager;
    private readonly ISessionManager _sessionManager = sessionManager;
    private readonly ITokenValidator _validator = validationProvider.CreateValidator(TokenTypes.Jwt);

    public async Task<Result> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.Request.PostLogoutRedirectUri))
        {
            return Results.BadRequest(new Error()
            {
                Code = ErrorTypes.OAuth.InvalidRequest,
                Description = "post_logout_redirect_uri is required."
            });
        }

        if (string.IsNullOrEmpty(request.Request.IdTokenHint))
        {
            return Results.BadRequest(new Error()
            {
                Code = ErrorTypes.OAuth.InvalidRequest,
                Description = "id_token_hint is required."
            });
        }

        var validationResult = await _validator.ValidateAsync(request.Request.IdTokenHint, cancellationToken);
        if (!validationResult.IsValid || validationResult.ClaimsPrincipal is null)
        {
            return Results.BadRequest(new Error()
            {
                Code = ErrorTypes.OAuth.InvalidRequest,
                Description = "id_token_hint is invalid."
            });
        }
        
        var principal = validationResult.ClaimsPrincipal;
        var sid = principal.Claims.FirstOrDefault(x => x.Type == AppClaimTypes.Sid);
        if (sid is null)
        {
            return Results.BadRequest(new Error()
            {
                Code = ErrorTypes.OAuth.InvalidRequest,
                Description = "id_token_hint is invalid."
            });
        }

        var session = await _sessionManager.FindByIdAsync(Guid.Parse(sid.Value), cancellationToken);
        if (session is null)
            return Results.InternalServerError(new Error()
            {
                Code = ErrorTypes.OAuth.ServerError,
                Description = "Invalid session."
            });

        ClientEntity? client;
        if (string.IsNullOrEmpty(request.Request.ClientId))
        {
            var audience = principal.Claims.First(x => x.Type == AppClaimTypes.Aud);
            client = await _clientManager.FindByAudienceAsync(audience.Value, cancellationToken);
        }
        else
        {
            client = await _clientManager.FindByIdAsync(request.Request.ClientId, cancellationToken);
        }

        if (client is null)
            return Results.Unauthorized(new Error()
            {
                Code = ErrorTypes.OAuth.InvalidClient,
                Description = "Invalid client."
            });
        
        var postLogoutRedirectUri = client.Uris.FirstOrDefault(x => x.Type == UriType.PostLogoutRedirect);
        if (postLogoutRedirectUri is null)
            return Results.BadRequest(new Error()
            {
                Code = ErrorTypes.OAuth.InvalidRequest,
                Description = "post_logout_redirect_uri is invalid."
            });

        var result = await _sessionManager.RemoveAsync(session, cancellationToken);
        if (!result.Succeeded) return result;

        var postLogoutRedirectUris = client.Uris
            .Where(x => x.Type == UriType.FrontChannelLogout)
            .Select(x => x.Uri)
            .ToList();
        
        var response = new LogoutResponse()
        {
            State = request.Request.State,
            PostLogoutRedirectUri = postLogoutRedirectUri.Uri,
            FrontChannelLogoutUris = postLogoutRedirectUris
        };
        
        return Results.Ok(response);
    }
}