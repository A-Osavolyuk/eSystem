using eSecurity.Server.Security.Authorization.Token;
using eSecurity.Server.Security.Cryptography.Hashing;
using eSystem.Core.Http.Constants;
using eSystem.Core.Http.Results;
using eSystem.Core.Security.Authorization.OAuth.Constants;
using eSystem.Core.Security.Authorization.OAuth.Revocation;

namespace eSecurity.Server.Features.Connect.Commands;

public record RevokeCommand(RevocationRequest Request) : IRequest<Result>;

public class RevokeCommandHandler(
    ITokenManager tokenManager,
    IHasherProvider hasherProvider) : IRequestHandler<RevokeCommand, Result>
{
    private readonly ITokenManager _tokenManager = tokenManager;
    private readonly IHasherProvider _hasherProvider = hasherProvider;

    public async Task<Result> Handle(RevokeCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.Request.Token))
            return Results.BadRequest(new Error
            {
                Code = ErrorTypes.OAuth.InvalidRequest,
                Description = "Token is missing."
            });

        OpaqueTokenType? tokenType = request.Request.TokenTypeHint switch
        {
            TokenTypeHints.AccessToken => OpaqueTokenType.AccessToken,
            TokenTypeHints.RefreshToken => OpaqueTokenType.RefreshToken,
            _ => null
        };

        var hasher = _hasherProvider.GetHasher(HashAlgorithm.Sha512);
        var incomingHash = hasher.Hash(request.Request.Token);
        var token = !tokenType.HasValue
            ? await _tokenManager.FindByHashAsync(incomingHash, cancellationToken)
            : await _tokenManager.FindByHashAsync(incomingHash, tokenType.Value, cancellationToken);

        if (token is null || token.Revoked) return Results.Ok();
        return await _tokenManager.RevokeAsync(token, cancellationToken);
    }
}