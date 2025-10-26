namespace eSystem.Auth.Api.Security.Authentication.SSO.Code;

public interface IAuthorizationCodeManager
{
    public ValueTask<AuthorizationCodeEntity?> FindByCodeAsync(string code, 
        CancellationToken cancellationToken = default);
    
    public ValueTask<Result> CreateAsync(AuthorizationCodeEntity code, 
        CancellationToken cancellationToken = default);

    public string Generate();
}