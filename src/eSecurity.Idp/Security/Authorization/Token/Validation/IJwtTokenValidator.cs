using TokenValidationResult = eSystem.Core.Server.Security.Authorization.OAuth.Token.Validation.TokenValidationResult;

namespace eSecurity.Idp.Security.Authorization.Token.Validation;

public interface IJwtTokenValidator
{
    public Task<TokenValidationResult> ValidateAsync(string token, CancellationToken cancellationToken = default);
}