using TokenValidationResult = eSystem.Core.Security.Authorization.OAuth.Token.Validation.TokenValidationResult;

namespace eSecurity.Server.Security.Authorization.OAuth.Token.Validation;

public interface IJwtTokenValidator
{
    public Task<TokenValidationResult> ValidateAsync(string token, CancellationToken cancellationToken = default);
}