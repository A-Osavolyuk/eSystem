using eSecurity.Core.Common.Requests;
using eSecurity.Server.Security.Authentication.Oidc.Client;
using eSecurity.Server.Security.Authentication.Oidc.Token;
using eSystem.Core.Security.Authentication.Oidc.Constants;
using eSystem.Core.Security.Authentication.Oidc.Revocation;

namespace eSecurity.Server.Features.Connect.Commands;

public record RevokeCommand(RevocationRequest Request) : IRequest<Result>;

public class RevokeCommandHandler(
    IClientManager clientManager,
    ITokenManager tokenManager) : IRequestHandler<RevokeCommand, Result>
{
    private readonly IClientManager _clientManager = clientManager;
    private readonly ITokenManager _tokenManager = tokenManager;

    public async Task<Result> Handle(RevokeCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.Request.Token))
            return Results.BadRequest(new Error()
            {
                Code = Errors.OAuth.InvalidRequest,
                Description = "Token is missing."
            });

        var tokenTypeHint = request.Request.TokenTypeHint;
        if (string.IsNullOrEmpty(tokenTypeHint) || tokenTypeHint == TokenTypeHints.RefreshToken)
        {
            var token = await _tokenManager.FindByTokenAsync(request.Request.Token, cancellationToken);
            if (token is null || token.Revoked) return Results.Ok();

            await _tokenManager.RevokeAsync(token, cancellationToken);
        }

        //TODO: Implement access token revocation
        return Results.Ok();
    }
}