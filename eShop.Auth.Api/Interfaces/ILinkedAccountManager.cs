namespace eShop.Auth.Api.Interfaces;

public interface ILinkedAccountManager
{
    
    public ValueTask<Result> CreateAsync(UserLinkedAccountEntity linkedAccount,
        CancellationToken cancellationToken = default);

    public ValueTask<Result> RemoveAsync(UserLinkedAccountEntity linkedAccount,
        CancellationToken cancellationToken = default);
    
    public ValueTask<Result> AllowAsync(UserLinkedAccountEntity linkedAccount,
        CancellationToken cancellationToken = default);
    
    public ValueTask<Result> DisallowAsync(UserLinkedAccountEntity linkedAccount,
        CancellationToken cancellationToken = default);
}