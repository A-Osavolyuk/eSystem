using eSecurity.Idp.Data.Entities;
using eSystem.Core.Primitives;

namespace eSecurity.Idp.Security.Authentication.Subject;

public interface ISubjectProvider
{
    public ValueTask<TypedResult<string>> GetSubjectAsync(UserEntity user, 
        ClientEntity client, CancellationToken cancellationToken = default);
}