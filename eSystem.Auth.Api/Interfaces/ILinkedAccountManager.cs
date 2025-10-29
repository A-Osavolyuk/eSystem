using eSystem.Auth.Api.Data.Entities;

namespace eSystem.Auth.Api.Interfaces;

public interface ILinkedAccountManager
{
    
    public ValueTask<Result> CreateAsync(UserLinkedAccountEntity linkedAccount,
        CancellationToken cancellationToken = default);

    public ValueTask<Result> RemoveAsync(UserLinkedAccountEntity linkedAccount,
        CancellationToken cancellationToken = default);
}