using eSecurity.Idp.Security.Authorization.Token;
using eSecurity.Idp.Security.Cryptography.Hashing;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;
using eSystem.Core.Security.Authorization.OAuth;
using eSystem.Core.Server.Binding;

namespace eSecurity.Idp.Features.Connect;

public sealed record RevocationCommand : IRequest<Result>
{
    [FromForm(Name = "token")] 
    public string Token { get; set; } = null!;
    
    [FromForm(Name = "token_type_hint")]
    public TokenTypeHint? TokenTypeHint { get; set; }
}

public class RevocationCommandHandler(
    ITokenManager tokenManager, 
    IHasherProvider hasherProvider) : IRequestHandler<RevocationCommand, Result>
{
    private readonly ITokenManager _tokenManager = tokenManager;
    private readonly IHasherProvider _hasherProvider = hasherProvider;

    public async Task<Result> Handle(RevocationCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.Token))
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.InvalidRequest,
                Description = "Token is missing."
            });
        }

        OpaqueTokenType? tokenType = request.TokenTypeHint switch
        {
            TokenTypeHint.AccessToken => OpaqueTokenType.AccessToken,
            TokenTypeHint.RefreshToken => OpaqueTokenType.RefreshToken,
            _ => null
        };

        var hasher = _hasherProvider.GetHasher(HashAlgorithm.Sha512);
        var incomingHash = hasher.Hash(request.Token);
        var token = !tokenType.HasValue
            ? await _tokenManager.FindByHashAsync(incomingHash, cancellationToken)
            : await _tokenManager.FindByHashAsync(incomingHash, tokenType.Value, cancellationToken);

        if (token is null || token.Revoked) 
            return Results.Success(SuccessCodes.Ok);
        
        return await _tokenManager.RevokeAsync(token, cancellationToken);
    }
}