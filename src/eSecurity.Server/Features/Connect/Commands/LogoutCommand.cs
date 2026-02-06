using eSecurity.Core.Common.Requests;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Logout;
using eSecurity.Server.Security.Authorization.Constants;
using eSecurity.Server.Security.Authorization.OAuth.Token.Validation;
using eSystem.Core.Http.Constants;
using eSystem.Core.Http.Results;
using eSystem.Core.Security.Authorization.OAuth.Token.Validation;
using eSystem.Core.Security.Identity.Claims;

namespace eSecurity.Server.Features.Connect.Commands;

public record LogoutCommand(LogoutRequest Request) : IRequest<Result>;

public class LogoutCommandHandler(
    ITokenValidationProvider validationProvider,
    ILogoutHandler logoutHandler) : IRequestHandler<LogoutCommand, Result>
{
    private readonly ILogoutHandler _logoutHandler = logoutHandler;
    private readonly ITokenValidator _validator = validationProvider.CreateValidator(TokenKind.Jwt);

    public async Task<Result> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.Request.PostLogoutRedirectUri))
        {
            return Results.BadRequest(new Error
            {
                Code = ErrorTypes.OAuth.InvalidRequest,
                Description = "post_logout_redirect_uri is required."
            });
        }

        if (string.IsNullOrEmpty(request.Request.IdTokenHint))
        {
            return Results.BadRequest(new Error
            {
                Code = ErrorTypes.OAuth.InvalidRequest,
                Description = "id_token_hint is required."
            });
        }

        var validationResult = await _validator.ValidateAsync(request.Request.IdTokenHint, cancellationToken);
        if (!validationResult.IsValid || validationResult.ClaimsPrincipal is null)
        {
            return Results.BadRequest(new Error
            {
                Code = ErrorTypes.OAuth.InvalidRequest,
                Description = "id_token_hint is invalid."
            });
        }
        
        var principal = validationResult.ClaimsPrincipal;
        var sidClaim = principal.Claims.FirstOrDefault(x => x.Type == AppClaimTypes.Sid);
        if (sidClaim is null)
        {
            return Results.BadRequest(new Error
            {
                Code = ErrorTypes.OAuth.InvalidRequest,
                Description = "id_token_hint is invalid."
            });
        }

        var context = new LogoutContext()
        {
            Sid = sidClaim.Value,
            State = request.Request.State,
            PostLogoutRedirectUri = request.Request.PostLogoutRedirectUri,
            Audience = string.IsNullOrWhiteSpace(request.Request.ClientId)
                ? principal.Claims.First(x => x.Type == AppClaimTypes.Aud).Value
                : request.Request.ClientId,
        };
        
        return await _logoutHandler.HandleAsync(context, cancellationToken);
    }
}