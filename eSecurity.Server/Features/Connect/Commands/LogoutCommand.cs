using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography.X509Certificates;
using eSecurity.Core.Common.Requests;
using eSecurity.Core.Common.Responses;
using eSecurity.Server.Data.Entities;
using eSecurity.Server.Security.Authentication.Oidc.Client;
using eSecurity.Server.Security.Authentication.Oidc.Session;
using eSecurity.Server.Security.Cryptography.Signing.Certificates;
using eSecurity.Server.Security.Cryptography.Tokens.Jwt;
using eSystem.Core.Security.Identity.Claims;
using eSystem.Core.Utilities.Query;

namespace eSecurity.Server.Features.Connect.Commands;

public record LogoutCommand(LogoutRequest Request) : IRequest<Result>;

public class LogoutCommandHandler(
    ICertificateProvider certificateProvider,
    IClientManager clientManager,
    ISessionManager sessionManager,
    IOptions<TokenOptions> tokenOptions) : IRequestHandler<LogoutCommand, Result>
{
    private readonly ICertificateProvider _certificateProvider = certificateProvider;
    private readonly IClientManager _clientManager = clientManager;
    private readonly ISessionManager _sessionManager = sessionManager;
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

        var handler = new JwtSecurityTokenHandler();
        var securityToken = handler.ReadJwtToken(request.Request.IdTokenHint);
        if (securityToken is null)
            return Results.BadRequest(new Error()
            {
                Code = Errors.OAuth.InvalidRequest,
                Description = "id_token_hint is invalid."
            });

        var kid = Guid.Parse(securityToken.Header.Kid);
        var certificate = await _certificateProvider.FindByIdAsync(kid, cancellationToken);
        if (certificate is null)
            return Results.InternalServerError(new Error()
            {
                Code = Errors.OAuth.ServerError,
                Description = "Public key not found."
            });

        var publicKey = certificate.Certificate.GetRSAPublicKey()!;
        var audiences = await _clientManager.GetAudiencesAsync(cancellationToken);

        var validationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidIssuer = _tokenOptions.Issuer,
            ValidateAudience = true,
            ValidAudiences = audiences,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new RsaSecurityKey(publicKey),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(5)
        };

        var principal = handler.ValidateToken(request.Request.IdTokenHint, validationParameters, out var validatedToken);
        if (validatedToken is null || principal is null)
            return Results.BadRequest(new Error()
            {
                Code = Errors.OAuth.InvalidRequest,
                Description = "id_token_hint is invalid."
            });

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
            client = await _clientManager.FindByIdAsync(audience.Value, cancellationToken);
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
        
        if (!client.HasPostLogoutRedirectUri(request.Request.PostLogoutRedirectUri))
            return Results.BadRequest(new Error()
            {
                Code = Errors.OAuth.InvalidRequest,
                Description = "post_logout_redirect_uri is invalid."
            });

        var result = await _sessionManager.RemoveAsync(session, cancellationToken);
        if (!result.Succeeded) return result;

        var postLogoutRedirectUris = client.PostLogoutRedirectUris
            .Select(x => x.Uri)
            .ToList();
        
        var response = new LogoutResponse()
        {
            State = request.Request.State,
            PostLogoutRedirectUris = postLogoutRedirectUris
        };
        
        return Results.Ok(response);
    }
}