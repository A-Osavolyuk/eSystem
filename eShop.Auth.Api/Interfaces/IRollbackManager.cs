namespace eShop.Auth.Api.Interfaces;

public interface IRollbackManager
{
    public ValueTask<RollbackEntity?> CommitAsync(UserEntity user, string value, 
        RollbackField field, CancellationToken cancellationToken);
    
    public ValueTask<Result> RollbackAsync(UserEntity user, string code, 
        RollbackField field, CancellationToken cancellationToken = default);
}