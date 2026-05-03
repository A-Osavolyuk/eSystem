using eSystem.Core.Primitives;

namespace eSecurity.Client.Security.Authentication.OpenIdConnect.Token;

public interface ITokenValidator
{
    public ValueTask<ValidationResult> ValidateAsync(string token, CancellationToken cancellationToken = default);
}