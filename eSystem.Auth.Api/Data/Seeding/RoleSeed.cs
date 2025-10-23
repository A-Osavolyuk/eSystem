using eSystem.Auth.Api.Entities;
using eSystem.Domain.Abstraction.Data.Seeding;

namespace eSystem.Auth.Api.Data.Seeding;

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
                Name = "Moderator",
                NormalizedName = "MODERATOR",
                Id = Guid.Parse("52425482-b33d-493f-acb2-aa520d73ce95")
            },
            new ()
            {
                Name = "Helper",
                NormalizedName = "HELPER",
                Id = Guid.Parse("8cfdff24-c06a-4d1b-89b6-a7d41501290a")
            },
            new ()
            {
                Name = "Technical support",
                NormalizedName = "TECHNICAL SUPPORT",
                Id = Guid.Parse("29b53acc-1990-4571-a4a3-66f835d36243")
            },
            new ()
            {
                Name = "User",
                NormalizedName = "USER",
                Id = Guid.Parse("270910a1-d582-4ce0-8b23-c8141d720064")
            },
            new ()
            {
                Name = "Seller",
                NormalizedName = "SELLER",
                Id = Guid.Parse("370b1519-9e73-4676-814c-e21433ceb424")
            }
        ];
    }
}