using eSystem.Auth.Api.Entities;
using eSystem.Domain.Common.Results;

namespace eSystem.Auth.Api.Interfaces;

public interface IAuthorizationManager
{
    public ValueTask<AuthorizationSessionEntity?> FindAsync(
        UserDeviceEntity device, CancellationToken cancellationToken);
    
    public ValueTask<Result> CreateAsync(UserDeviceEntity device, CancellationToken cancellationToken);
    public ValueTask<Result> RemoveAsync(AuthorizationSessionEntity session, CancellationToken cancellationToken);
}