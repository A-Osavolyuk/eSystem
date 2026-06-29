using eSecurity.Idp.Data.Entities;
using eSystem.Core.Primitives;

namespace eSecurity.Idp.Security.Authentication.Subject;

public interface ISubjectProvider
{
    ValueTask<TypedResult<string>> GetSubjectAsync(Guid userId, 
        Guid clientId, CancellationToken cancellationToken = default);
}