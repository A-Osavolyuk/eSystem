using eSecurity.Core.Common.Requests;
using eSecurity.Server.Data.Entities;
using eSecurity.Server.Security.Authentication.Oidc.Client;
using eSecurity.Server.Security.Authentication.Oidc.Token;
using eSecurity.Server.Security.Cryptography.Hashing;
using eSystem.Core.Security.Authentication.Oidc.Revocation;

namespace eSecurity.Server.Features.Connect.Commands;

public record RevokeCommand(RevocationRequest Request) : IRequest<Result>;

public class RevokeCommandHandler(
    ITokenManager tokenManager,
    IHasherFactory hasherFactory) : IRequestHandler<RevokeCommand, Result>
{
    private readonly ITokenManager _tokenManager = tokenManager;
    private readonly IHasherFactory _hasherFactory = hasherFactory;

    public async Task<Result> Handle(RevokeCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.Request.Token))
            return Results.BadRequest(new Error()
            {
                Code = Errors.OAuth.InvalidRequest,
                Description = "Token is missing."
            });

        OpaqueTokenType? tokenType = request.Request.TokenTypeHint switch
        {
            TokenTypeHints.AccessToken => OpaqueTokenType.AccessToken,
            TokenTypeHints.RefreshToken => OpaqueTokenType.RefreshToken,
            _ => null
        };

        var hasher = _hasherFactory.Create(HashAlgorithm.Sha512);
        var incomingHash = hasher.Hash(request.Request.Token);
        var token = !tokenType.HasValue
            ? await _tokenManager.FindByTokenAsync(incomingHash, cancellationToken)
            : await _tokenManager.FindByTokenAsync(incomingHash, tokenType.Value, cancellationToken);

        if (token is null || token.Revoked) return Results.Ok();
        return await _tokenManager.RevokeAsync(token, cancellationToken);
    }
}