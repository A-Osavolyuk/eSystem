using eSecurity.Data.Entities;

namespace eSecurity.Security.Authorization.OAuth.LinkedAccount;

public interface ILinkedAccountManager
{
    
    public ValueTask<Result> CreateAsync(UserLinkedAccountEntity linkedAccount,
        CancellationToken cancellationToken = default);

    public ValueTask<Result> RemoveAsync(UserLinkedAccountEntity linkedAccount,
        CancellationToken cancellationToken = default);
}