using eSecurity.Server.Data.Entities;

namespace eSecurity.Server.Security.Authorization.Roles;

public interface IRoleManager
{
    public ValueTask<List<UserRoleEntity>> GetAllAsync(UserEntity user, CancellationToken cancellationToken = default);
    public ValueTask<RoleEntity?> FindByNameAsync(string name, CancellationToken cancellationToken = default);

    public ValueTask<Result> AssignAsync(UserEntity user, RoleEntity role,
        CancellationToken cancellationToken = default);
}