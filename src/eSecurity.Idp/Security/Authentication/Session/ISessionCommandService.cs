using eSecurity.Idp.Data.Entities;
using eSystem.Core.Primitives;

namespace eSecurity.Idp.Security.Authentication.Session;

public interface ISessionCommandService
{
    ValueTask<Result> CreateAsync(SessionEntity entity, CancellationToken cancellationToken = default);

    ValueTask<Result> RemoveAsync(SessionEntity entity, CancellationToken cancellationToken = default);
}