using eSecurity.Server.Security.Authorization.OAuth.Token;
using eSecurity.Server.Security.Cryptography.Hashing;
using eSystem.Core.Binding;
using eSystem.Core.Mediator;
using eSystem.Core.Primitives;
using eSystem.Core.Security.Authorization.OAuth;
using eSystem.Core.Security.Authorization.OAuth.Revocation;

namespace eSecurity.Server.Features.Connect.Commands;

public record RevokeCommand(IFormCollection Form) : IRequest<Result>;

public class RevokeCommandHandler(
    ITokenManager tokenManager,
    IHasherProvider hasherProvider,
    IFormBindingProvider bindingProvider) : IRequestHandler<RevokeCommand, Result>
{
    private readonly ITokenManager _tokenManager = tokenManager;
    private readonly IHasherProvider _hasherProvider = hasherProvider;
    private readonly IFormBindingProvider _bindingProvider = bindingProvider;

    public async Task<Result> Handle(RevokeCommand request, CancellationToken cancellationToken)
    {
        var binder = _bindingProvider.GetRequiredBinder<RevocationRequest>();
        var bindingResult = await binder.BindAsync(request.Form, cancellationToken);
        if (!bindingResult.Succeeded || !bindingResult.TryGetValue(out var revocationRequest))
        {
            var error = bindingResult.GetError();
            return Results.BadRequest(error);
        }
        
        if (string.IsNullOrEmpty(revocationRequest.Token))
            return Results.BadRequest(new Error
            {
                Code = ErrorCode.InvalidRequest,
                Description = "Token is missing."
            });

        OpaqueTokenType? tokenType = revocationRequest.TokenTypeHint switch
        {
            TokenTypeHint.AccessToken => OpaqueTokenType.AccessToken,
            TokenTypeHint.RefreshToken => OpaqueTokenType.RefreshToken,
            _ => null
        };

        var hasher = _hasherProvider.GetHasher(HashAlgorithm.Sha512);
        var incomingHash = hasher.Hash(revocationRequest.Token);
        var token = !tokenType.HasValue
            ? await _tokenManager.FindByHashAsync(incomingHash, cancellationToken)
            : await _tokenManager.FindByHashAsync(incomingHash, tokenType.Value, cancellationToken);

        if (token is null || token.Revoked) return Results.Ok();
        return await _tokenManager.RevokeAsync(token, cancellationToken);
    }
}