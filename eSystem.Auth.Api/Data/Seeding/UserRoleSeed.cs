using eSystem.Auth.Api.Entities;
using eSystem.Domain.Abstraction.Data.Seeding;

namespace eSystem.Auth.Api.Data.Seeding;

public class UserRoleSeed : Seed<UserRoleEntity>
{
    public override List<UserRoleEntity> Get()
    {
        return [
            new ()
            {
                RoleId = Guid.Parse("e6d15d97-b803-435a-9dc2-a7c45c08a1af"),
                UserId = Guid.Parse("188c7286-b0b9-4cb1-8f7f-503c6349bc65")
            },
            new ()
            {
                RoleId = Guid.Parse("270910a1-d582-4ce0-8b23-c8141d720064"),
                UserId = Guid.Parse("188c7286-b0b9-4cb1-8f7f-503c6349bc65")
            }
        ];
    }
}