namespace eShop.Auth.Api.Interfaces;

public interface IRoleManager
{
    public ValueTask<List<RoleEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    public ValueTask<RoleEntity?> FindByNameAsync(string name, CancellationToken cancellationToken = default);
    public ValueTask<RoleEntity?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
    public ValueTask<Result> DeleteAsync(RoleEntity entity, CancellationToken cancellationToken = default);
    public ValueTask<Result> CreateAsync(RoleEntity entity, CancellationToken cancellationToken = default);
    public ValueTask<Result> UpdateAsync(RoleEntity entity, CancellationToken cancellationToken = default);
    public ValueTask<Result> AssignAsync(UserEntity user, RoleEntity role, CancellationToken cancellationToken = default);
    public ValueTask<Result> UnassignAsync(UserEntity user, RoleEntity role, CancellationToken cancellationToken = default);
    public ValueTask<Result> UnassignAsync(UserEntity user, CancellationToken cancellationToken = default);
}