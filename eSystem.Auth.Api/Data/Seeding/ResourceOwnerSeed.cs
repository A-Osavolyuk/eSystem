using eSystem.Domain.Abstraction.Data.Seeding;

namespace eSystem.Auth.Api.Data.Seeding;

public class ResourceOwnerSeed : Seed<ResourceOwnerEntity>
{
    public override List<ResourceOwnerEntity> Get()
    {
        return
        [
            new ResourceOwnerEntity()
            {
                Id = Guid.Parse("c849aa5a-9a79-4d7a-84f3-e6835f05d242"),
                Name = "eSecurity"
            },
            new ResourceOwnerEntity()
            {
                Id = Guid.Parse("fc8dcb62-a9ca-406c-a22b-84e6fc8f94d7"),
                Name = "eStorage"
            },
            new ResourceOwnerEntity()
            {
                Id = Guid.Parse("a3e10874-01a0-4bb5-80a0-a647e31126ca"),
                Name = "eShop"
            },
            new ResourceOwnerEntity()
            {
                Id = Guid.Parse("aa1c25e4-67db-4b17-9c97-5e9f7bd4ede7"),
                Name = "eBank"
            },
            new ResourceOwnerEntity()
            {
                Id = Guid.Parse("ab530f2e-cc35-4153-8898-35c9b19442cd"),
                Name = "eCinema"
            },
            new ResourceOwnerEntity()
            {
                Id = Guid.Parse("06c697a0-321b-48ce-a109-d36cdb3b5f6f"),
                Name = "eMusic"
            }
        ];
    }
}