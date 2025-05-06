namespace eShop.Auth.Api.Interfaces;

public interface IRoleManager
{
    public ValueTask<bool> ExistsAsync(string name, CancellationToken cancellationToken = default);
    public ValueTask<List<RoleEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    public ValueTask<List<RoleEntity>> GetByUserAsync(UserEntity user, CancellationToken cancellationToken = default);
    public ValueTask<RoleEntity?> FindByNameAsync(string name, CancellationToken cancellationToken = default);
    public ValueTask<RoleEntity?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
    public ValueTask DeleteAsync(RoleEntity entity, CancellationToken cancellationToken = default);
    public ValueTask CreateAsync(RoleEntity entity, CancellationToken cancellationToken = default);
    public ValueTask UpdateAsync(RoleEntity entity, CancellationToken cancellationToken = default);
}