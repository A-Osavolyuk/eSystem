using eSystem.Auth.Api.Entities;
using eSystem.Domain.Abstraction.Data.Seeding;

namespace eSystem.Auth.Api.Data.Seeding;

public class UserPermissionSeed : Seed<UserPermissionsEntity>
{
    public override List<UserPermissionsEntity> Get()
    {
        return [];
    }
}