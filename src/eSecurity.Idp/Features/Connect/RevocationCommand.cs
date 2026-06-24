using eSecurity.Idp.Security.Authorization.Token;
using eSecurity.Idp.Security.Cryptography.Hashing;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;
using eSystem.Core.Security.Authorization.OAuth;

namespace eSecurity.Idp.Features.Connect;

public sealed class RevocationCommand : IRequest<Result>
{
    [FromForm(Name = "token")] 
    public string Token { get; set; } = null!;
    
    [FromForm(Name = "token_type_hint")]
    public TokenTypeHint? TokenTypeHint { get; set; }
}

public sealed class RevocationCommandHandler(
    ITokenManager tokenManager, 
    IHasherProvider hasherProvider) : IRequestHandler<RevocationCommand, Result>
{
    private readonly ITokenManager _tokenManager = tokenManager;
    private readonly IHasherProvider _hasherProvider = hasherProvider;

    public async Task<Result> Handle(RevocationCommand request, CancellationToken cancellationToken)
    {
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

public sealed class RevocationCommandValidator : IRequestValidator<RevocationCommand>
{
    public ValueTask<Result> Validate(RevocationCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.Token))
        {
            return ValueTask.FromResult(Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.InvalidRequest,
                Description = "'token' is required"
            }));
        }

        return ValueTask.FromResult(Results.Success(SuccessCodes.Ok));
    }
}