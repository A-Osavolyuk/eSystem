namespace eShop.Auth.Api.Interfaces;

public interface ILoginMethodManager
{
    public ValueTask<List<LoginMethodEntity>> GetAllAsync(
        CancellationToken cancellationToken = default);
    public ValueTask<LoginMethodEntity?> FindAsync(LoginType type, 
        CancellationToken cancellationToken = default);
    public ValueTask<Result> CreateAsync(UserEntity user, LoginType type, 
        CancellationToken cancellationToken = default);
    
    public ValueTask<Result> RemoveAsync(UserLoginMethodEntity entity, 
        CancellationToken cancellationToken = default);
}