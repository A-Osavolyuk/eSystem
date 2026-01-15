namespace eCinema.Server.Security.Authentication.Oidc.Token;

public interface ITokenValidator
{
    public Task<Result> ValidateAsync(string token, CancellationToken cancellationToken = default);
}