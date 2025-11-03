using eSystem.Auth.Api.Data.Entities;

namespace eSystem.Auth.Api.Security.Authentication.ODIC.Code;

public interface IAuthorizationCodeManager
{
    public ValueTask<AuthorizationCodeEntity?> FindByCodeAsync(string code, 
        CancellationToken cancellationToken = default);
    
    public ValueTask<Result> CreateAsync(AuthorizationCodeEntity code, 
        CancellationToken cancellationToken = default);
    
    public ValueTask<Result> UseAsync(AuthorizationCodeEntity code, 
        CancellationToken cancellationToken = default);

    public string Generate();
}