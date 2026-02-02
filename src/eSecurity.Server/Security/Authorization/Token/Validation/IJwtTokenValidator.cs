using TokenValidationResult = eSystem.Core.Security.Authentication.OpenIdConnect.Token.Validation.TokenValidationResult;

namespace eSecurity.Server.Security.Authorization.Token.Validation;

public interface IJwtTokenValidator
{
    public Task<TokenValidationResult> ValidateAsync(string token, CancellationToken cancellationToken = default);
}