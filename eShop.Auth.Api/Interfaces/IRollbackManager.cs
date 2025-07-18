namespace eShop.Auth.Api.Interfaces;

public interface IRollbackManager
{
    public ValueTask<RollbackEntity?> FindAsync(UserEntity user, string code,
        RollbackField field, CancellationToken cancellationToken);
    
    public ValueTask<RollbackEntity?> FindAsync(UserEntity user,
        RollbackField field, CancellationToken cancellationToken);
    
    public ValueTask<RollbackEntity?> SaveAsync(UserEntity user, string value, 
        RollbackField field, CancellationToken cancellationToken);
    
    public ValueTask<Result> RemoveAsync(RollbackEntity entity, CancellationToken cancellationToken);
}