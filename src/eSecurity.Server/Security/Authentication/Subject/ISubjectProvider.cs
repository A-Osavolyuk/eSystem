using eSecurity.Server.Data.Entities;

namespace eSecurity.Server.Security.Authentication.Subject;

public interface ISubjectProvider
{
    public ValueTask<TypedResult<string>> GetSubjectAsync(UserEntity user, 
        ClientEntity client, CancellationToken cancellationToken = default);
}