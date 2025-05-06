namespace eShop.Auth.Api.Interfaces;

public interface IRoleManager
{
    public ValueTask<List<RoleEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    public ValueTask<List<RoleEntity>> GetAllAsync(UserEntity user, CancellationToken cancellationToken = default);
}