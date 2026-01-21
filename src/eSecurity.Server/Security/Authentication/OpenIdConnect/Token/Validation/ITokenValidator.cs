namespace eSecurity.Server.Security.Authentication.OpenIdConnect.Token.Validation;

public interface ITokenValidator
{
    public Task<TokenValidationResult> ValidateAsync(string token, CancellationToken cancellationToken = default);
}