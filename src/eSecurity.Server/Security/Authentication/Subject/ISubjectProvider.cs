using eSecurity.Server.Data.Entities;
using eSystem.Core.Primitives;

namespace eSecurity.Server.Security.Authentication.Subject;

public interface ISubjectProvider
{
    public ValueTask<TypedResult<string>> GetSubjectAsync(UserEntity user, 
        ClientEntity client, CancellationToken cancellationToken = default);
}