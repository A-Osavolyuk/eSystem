using eSecurity.Idp.Security.Authorization.Token;
using eSecurity.Idp.Security.Cryptography.Hashing;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;
using eSystem.Core.Security.Authorization.OAuth;

namespace eSecurity.Idp.Features.Connect;

public sealed class RevocationCommand : IRequest<Result>
{
    [FromForm(Name = "token")] public string Token { get; set; } = null!;

    [FromForm(Name = "token_type_hint")] public TokenTypeHint? TokenTypeHint { get; set; }
}

public sealed class RevocationCommandHandler(
    ITokenQueryService tokenQueryService,
    ITokenCommandService tokenCommandService) : IRequestHandler<RevocationCommand, Result>
{
    private readonly ITokenQueryService _tokenQueryService = tokenQueryService;
    private readonly ITokenCommandService _tokenCommandService = tokenCommandService;

    public async Task<Result> Handle(RevocationCommand request, CancellationToken cancellationToken)
    {
        OpaqueTokenType? tokenType = request.TokenTypeHint switch
        {
            TokenTypeHint.AccessToken => OpaqueTokenType.AccessToken,
            TokenTypeHint.RefreshToken => OpaqueTokenType.RefreshToken,
            _ => null
        };
        
        var token = !tokenType.HasValue
            ? await _tokenQueryService.GetByTokenAsync(request.Token, cancellationToken)
            : await _tokenQueryService.GetByTokenAsync(request.Token, tokenType.Value, cancellationToken);

        if (token is null || token.Revoked)
            return Results.Success(SuccessCodes.Ok);

        return await _tokenCommandService.RevokeAsync(token.Id, cancellationToken);
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