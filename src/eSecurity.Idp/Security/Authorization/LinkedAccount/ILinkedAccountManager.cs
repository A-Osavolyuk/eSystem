using eSecurity.Core.Security.Authorization.OAuth;
using eSecurity.Idp.Data.Entities;
using eSystem.Core.Primitives;

namespace eSecurity.Idp.Security.Authorization.LinkedAccount;

public interface ILinkedAccountManager
{
    public ValueTask<UserLinkedAccountEntity?> GetAsync(UserEntity user,
        LinkedAccountType type, CancellationToken cancellationToken = default);
    
    public ValueTask<List<UserLinkedAccountEntity>> GetAllAsync(UserEntity user,
        CancellationToken cancellationToken = default);

    public ValueTask<Result> CreateAsync(UserLinkedAccountEntity linkedAccount,
        CancellationToken cancellationToken = default);

    public ValueTask<Result> RemoveAsync(UserLinkedAccountEntity linkedAccount,
        CancellationToken cancellationToken = default);

    public ValueTask<bool> HasAsync(UserEntity user, CancellationToken cancellationToken = default);

    public ValueTask<bool> HasAsync(UserEntity user, LinkedAccountType type,
        CancellationToken cancellationToken = default);
}