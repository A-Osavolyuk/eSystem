namespace eShop.Auth.Api.Interfaces;

public interface IRollbackManager
{
    public ValueTask<Result> SaveAsync(UserEntity user, string value, 
        RollbackField field, RollbackAction action, CancellationToken cancellationToken);
}