namespace eShop.Auth.Api.Interfaces;

public interface IRollbackManager
{
    public ValueTask<RollbackEntity?> FindAsync(UserEntity user, string code,
        RollbackField field, RollbackAction action, CancellationToken cancellationToken);
    
    public ValueTask<Result> SaveAsync(UserEntity user, string value, 
        RollbackField field, RollbackAction action, CancellationToken cancellationToken);
    
    public ValueTask<Result> RemoveAsync(RollbackEntity entity, CancellationToken cancellationToken);
}