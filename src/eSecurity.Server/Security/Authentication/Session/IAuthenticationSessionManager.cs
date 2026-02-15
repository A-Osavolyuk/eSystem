using eSecurity.Server.Data.Entities;
using eSystem.Core.Http.Results;

namespace eSecurity.Server.Security.Authentication.Session;

public interface IAuthenticationSessionManager
{
    public ValueTask<AuthenticationSessionEntity?> FindByIdAsync(Guid id, 
        CancellationToken cancellationToken = default);
    
    public ValueTask<Result> CreateAsync(AuthenticationSessionEntity entity, 
        CancellationToken cancellationToken = default);
    
    public ValueTask<Result> UpdateAsync(AuthenticationSessionEntity entity, 
        CancellationToken cancellationToken = default);
}