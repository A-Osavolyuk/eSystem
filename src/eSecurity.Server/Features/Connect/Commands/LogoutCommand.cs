using System.Security.Claims;
using eSecurity.Core.Common.Requests;
using eSecurity.Core.Common.Responses;
using eSecurity.Server.Data.Entities;
using eSecurity.Server.Security.Authentication.Oidc.Client;
using eSecurity.Server.Security.Authentication.Oidc.Session;
using eSecurity.Server.Security.Authentication.Oidc.Token;
using eSystem.Core.Security.Identity.Claims;

namespace eSecurity.Server.Features.Connect.Commands;

public record LogoutCommand(LogoutRequest Request) : IRequest<Result>;

public class LogoutCommandHandler(
    IClientManager clientManager,
    ISessionManager sessionManager,
    ITokenValidator tokenValidator,
    IOptions<TokenOptions> tokenOptions) : IRequestHandler<LogoutCommand, Result>
{
    private readonly IClientManager _clientManager = clientManager;
    private readonly ISessionManager _sessionManager = sessionManager;
    private readonly ITokenValidator _tokenValidator = tokenValidator;
    private readonly TokenOptions _tokenOptions = tokenOptions.Value;

    public async Task<Result> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.Request.PostLogoutRedirectUri))
            return Results.BadRequest(new Error()
            {
                Code = Errors.OAuth.InvalidRequest,
                Description = "post_logout_redirect_uri is required."
            });

        if (string.IsNullOrEmpty(request.Request.IdTokenHint))
            return Results.BadRequest(new Error()
            {
                Code = Errors.OAuth.InvalidRequest,
                Description = "id_token_hint is required."
            });

        var validationResult = await _tokenValidator.ValidateAsync(request.Request.IdTokenHint, cancellationToken);
        if (!validationResult.Succeeded) return validationResult;
        
        var principal = validationResult.Get<ClaimsPrincipal>();
        var sid = principal.Claims.FirstOrDefault(x => x.Type == AppClaimTypes.Sid);
        if (sid is null)
            return Results.BadRequest(new Error()
            {
                Code = Errors.OAuth.InvalidRequest,
                Description = "id_token_hint is invalid."
            });

        var session = await _sessionManager.FindByIdAsync(Guid.Parse(sid.Value), cancellationToken);
        if (session is null)
            return Results.InternalServerError(new Error()
            {
                Code = Errors.OAuth.ServerError,
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
                Code = Errors.OAuth.InvalidClient,
                Description = "Invalid client."
            });
        
        if (!client.HasUri(request.Request.PostLogoutRedirectUri, UriType.PostLogoutRedirect))
            return Results.BadRequest(new Error()
            {
                Code = Errors.OAuth.InvalidRequest,
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
            Uris = postLogoutRedirectUris
        };
        
        return Results.Ok(response);
    }
}