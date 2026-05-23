using eSecurity.Idp.Data.Entities;
using eSystem.Core.Primitives;

namespace eSecurity.Idp.Security.Authentication.EndSession;

public interface IEndSessionManager
{
    public ValueTask<EndSessionRequestEntity?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
    public ValueTask<EndSessionRequestEntity?> FindByIdAsync(string id, CancellationToken cancellationToken = default);

    public ValueTask<Result> CreateAsync(EndSessionRequestEntity entity, CancellationToken cancellationToken = default);
    public ValueTask<Result> UpdateAsync(EndSessionRequestEntity entity, CancellationToken cancellationToken = default);
    public ValueTask<Result> RemoveAsync(EndSessionRequestEntity entity, CancellationToken cancellationToken = default);
}