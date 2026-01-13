using eSecurity.Server.Data.Entities;
using eSystem.Core.Data.Seeding;

namespace eSecurity.Server.Data.Seeding;

public class RoleSeed : Seed<RoleEntity>
{
    public override List<RoleEntity> Get()
    {
        return
        [
            new ()
            {
                Name = "Admin",
                NormalizedName = "ADMIN",
                Id = Guid.Parse("e6d15d97-b803-435a-9dc2-a7c45c08a1af")
            },
            new ()
            {
                Name = "User",
                NormalizedName = "USER",
                Id = Guid.Parse("270910a1-d582-4ce0-8b23-c8141d720064")
            }
        ];
    }
}