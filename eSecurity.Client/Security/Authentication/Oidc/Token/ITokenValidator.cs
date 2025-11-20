namespace eSecurity.Client.Security.Authentication.Oidc.Token;

public interface ITokenValidator
{
    public ValueTask<Result> ValidateAsync(string token);
}