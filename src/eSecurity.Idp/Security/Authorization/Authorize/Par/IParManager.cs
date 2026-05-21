using eSecurity.Idp.Data.Entities;
using eSystem.Core.Primitives;

namespace eSecurity.Idp.Security.Authorization.Authorize.Par;

public interface IParManager
{
    public ValueTask<PushedAuthorizationRequestEntity?> FindByIdAsync(Guid id, CancellationToken cancellationToken);
    public ValueTask<PushedAuthorizationRequestEntity?> FindByRequestUriAsync(string requestUri, 
        CancellationToken cancellationToken);
    
    public ValueTask<Result> CreateAsync(PushedAuthorizationRequestEntity entity, CancellationToken cancellationToken);
    public ValueTask<Result> UpdateAsync(PushedAuthorizationRequestEntity entity, CancellationToken cancellationToken);
    public ValueTask<Result> RemoveAsync(PushedAuthorizationRequestEntity entity, CancellationToken cancellationToken);
}