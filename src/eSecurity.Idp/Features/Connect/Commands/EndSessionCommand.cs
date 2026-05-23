using eSecurity.Idp.Data.Entities;
using eSecurity.Idp.Security.Authentication.EndSession;
using eSecurity.Idp.Security.Authentication.OpenIdConnect.Client;
using eSecurity.Idp.Security.Authentication.OpenIdConnect.Session;
using eSecurity.Idp.Security.Authorization.Constants;
using eSecurity.Idp.Security.Authorization.OAuth.Token.Validation;
using eSecurity.Idp.Security.Identity.User;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;
using eSystem.Core.Security.Identity.Claims;
using eSystem.Core.Server.Security.Authorization.OAuth.Token.Validation;
using eSystem.Core.Utilities.Query;

namespace eSecurity.Idp.Features.Connect.Commands;

public sealed record EndSessionCommand(EndSessionRequest Request) : IRequest<Result>;

public sealed class EndSessionCommandHandler(
    IEndSessionManager endSessionManager,
    IOptions<EndSessionOptions> options,
    IClientManager clientManager,
    IUserManager userManager,
    ISessionManager sessionManager,
    ISessionAccessor sessionAccessor,
    ITokenValidationProvider tokenValidationProvider) : IRequestHandler<EndSessionCommand, Result>
{
    private readonly IEndSessionManager _endSessionManager = endSessionManager;
    private readonly IClientManager _clientManager = clientManager;
    private readonly IUserManager _userManager = userManager;
    private readonly ISessionManager _sessionManager = sessionManager;
    private readonly ISessionAccessor _sessionAccessor = sessionAccessor;
    private readonly EndSessionOptions _options = options.Value;
    private readonly ITokenValidator _tokenValidator = tokenValidationProvider.CreateValidator(TokenKind.Jwt);

    public async Task<Result> Handle(EndSessionCommand request, CancellationToken cancellationToken = default)
    {
        var redirectUri = _options.FallbackUrl;
        var logoutUri = _options.LogoutUrl;
        if (string.IsNullOrEmpty(redirectUri) || string.IsNullOrEmpty(logoutUri))
            throw new InvalidOperationException("End session flow was not configured correctly");

        var uiLocales = request.Request.UiLocales?.Split(' ') ?? [];
        if (string.IsNullOrEmpty(request.Request.IdTokenHint) && string.IsNullOrEmpty(request.Request.ClientId))
        {
            return Fallback(redirectUri, ErrorCode.InvalidRequest,
                "either client_id or id_token_hint is required", request.Request.State);
        }

        if (string.IsNullOrEmpty(request.Request.IdTokenHint) && string.IsNullOrEmpty(request.Request.LogoutHint))
        {
            return Fallback(redirectUri, ErrorCode.InvalidRequest,
                "either logout_hint or id_token_hint is required", request.Request.State);
        }

        var sessionCookie = _sessionAccessor.GetCookie();
        if (sessionCookie is null)
        {
            return Fallback(redirectUri, ErrorCode.InvalidRequest,
                "No active session was found", request.Request.State);
        }

        var session = await _sessionManager.FindByIdAsync(sessionCookie.SessionId, cancellationToken);
        if (session is null)
        {
            return Fallback(redirectUri, ErrorCode.InvalidRequest,
                "No active session was found", request.Request.State);
        }

        UserEntity? user;
        ClientEntity? client;
        
        if (string.IsNullOrEmpty(request.Request.IdTokenHint))
        {
            client = await _clientManager.FindByIdAsync(request.Request.ClientId!, cancellationToken);
            if (client is null)
            {
                return Fallback(redirectUri, ErrorCode.InvalidRequest,
                    "client_id is invalid", request.Request.State);
            }

            if (!string.IsNullOrEmpty(request.Request.PostLogoutRedirectUri))
            {
                if (!client.HasUri(request.Request.PostLogoutRedirectUri, UriType.PostLogoutRedirect))
                {
                    return Fallback(redirectUri, ErrorCode.InvalidRequest,
                        "post_logout_redirect_uri is invalid", request.Request.PostLogoutRedirectUri);
                }

                redirectUri = request.Request.PostLogoutRedirectUri;
            }

            user = await _userManager.FindByLoginAsync(request.Request.LogoutHint!, cancellationToken);
            if (user is null || session.UserId != user.Id)
            {
                return Fallback(redirectUri, ErrorCode.InvalidRequest,
                    "logout_hint is invalid", request.Request.State);
            }
        }
        else
        {
            var validationResult = await _tokenValidator.ValidateAsync(request.Request.IdTokenHint, cancellationToken);
            if (!validationResult.IsValid || validationResult.ClaimsPrincipal is null)
            {
                return Fallback(redirectUri, ErrorCode.InvalidRequest,
                    "id_token_hint is invalid", request.Request.State);
            }

            var claimsPrincipal = validationResult.ClaimsPrincipal;
            var sidClaim = claimsPrincipal.FindFirst(AppClaimTypes.Sid);
            var audClaim = claimsPrincipal.FindFirst(AppClaimTypes.Aud);
            var subClaim = claimsPrincipal.FindFirst(AppClaimTypes.Sub);
            
            if (sidClaim is null || session.Id != Guid.Parse(sidClaim.Value) || audClaim is null || subClaim is null)
            {
                return Fallback(redirectUri, ErrorCode.InvalidRequest,
                    "id_token_hint is invalid", request.Request.State);
            }
            
            user = await _userManager.FindBySubjectAsync(subClaim.Value, cancellationToken);
            if (user is null)
            {
                return Fallback(redirectUri, ErrorCode.InvalidRequest,
                    "id_token_hint is invalid", request.Request.State);
            }

            client = await _clientManager.FindByIdAsync(audClaim.Value, cancellationToken);
            if (client is null)
            {
                return Fallback(redirectUri, ErrorCode.InvalidRequest,
                    "id_token_hint is invalid", request.Request.State);
            }
            
            if (!string.IsNullOrEmpty(request.Request.PostLogoutRedirectUri))
            {
                if (!client.HasUri(request.Request.PostLogoutRedirectUri, UriType.PostLogoutRedirect))
                {
                    return Fallback(redirectUri, ErrorCode.InvalidRequest,
                        "post_logout_redirect_uri is invalid", request.Request.PostLogoutRedirectUri);
                }

                redirectUri = request.Request.PostLogoutRedirectUri;
            }
        }

        if (!await _sessionManager.OwnClientAsync(session, client, cancellationToken))
        {
            return Fallback(redirectUri, ErrorCode.InvalidRequest,
                "client_id is invalid", request.Request.State);
        }

        var endSessionRequestEntity = new EndSessionRequestEntity()
        {
            Id = Guid.CreateVersion7(),
            UserId = user.Id,
            ClientId = client.Id,
            SessionId = session.Id,
            PostLogoutRedirectUri = request.Request.PostLogoutRedirectUri,
            State = request.Request.State,
            Status = EndSessionStatus.Pending,
            ExpiredAt = DateTimeOffset.UtcNow.Add(_options.Timestamp)
        };

        endSessionRequestEntity.AddUiLocales(uiLocales);

        var result = await _endSessionManager.CreateAsync(endSessionRequestEntity, cancellationToken);
        if (!result.Succeeded)
        {
            var error = result.GetError();
            return Fallback(redirectUri, error.Code, error.Description, request.Request.State);
        }

        var builder = QueryBuilder.Create()
            .WithUri(logoutUri)
            .WithQueryParam("end_session_request_id", endSessionRequestEntity.Id)
            .WithQueryParam("user_hint", user.Username);

        if (!string.IsNullOrEmpty(request.Request.UiLocales))
            builder.WithQueryParam("ui_locales", request.Request.UiLocales);
            
        if (!string.IsNullOrEmpty(request.Request.State))
            builder.WithQueryParam("state", request.Request.State);

        return Results.Redirect(RedirectionCode.Found, builder.Build());
    }

    private static Result Fallback(string uri, ErrorCode error, string description, string? state = null)
    {
        var builder = QueryBuilder.Create()
            .WithUri(uri)
            .WithQueryParam("error", error)
            .WithQueryParam("error_description", description);

        if (!string.IsNullOrEmpty(state))
            builder.WithQueryParam("state", state);

        return Results.Redirect(RedirectionCode.Found, builder.Build());
    }
}