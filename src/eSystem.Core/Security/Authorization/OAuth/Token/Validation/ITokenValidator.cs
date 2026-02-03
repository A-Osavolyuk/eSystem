namespace eSystem.Core.Security.Authorization.OAuth.Token.Validation;

public interface ITokenValidator
{
    public Task<TokenValidationResult> ValidateAsync(string token, CancellationToken cancellationToken = default);
}