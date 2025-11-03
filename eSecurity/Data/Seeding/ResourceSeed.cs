using eSecurity.Data.Entities;
using eSystem.Core.Data.Seeding;

namespace eSecurity.Data.Seeding;

public class ResourceSeed : Seed<ResourceEntity>
{
    public override List<ResourceEntity> Get()
    {
        return
        [
            new ResourceEntity()
            {
                Id = Guid.Parse("1a06eabb-3354-4b03-be52-ab42743eaa97"),
                OwnerId = Guid.Parse("c849aa5a-9a79-4d7a-84f3-e6835f05d242"),
                Name = "Account",
                CreateDate = null,
                UpdateDate = null
            },
            new ResourceEntity()
            {
                Id = Guid.Parse("e3fef8c4-0105-419a-b889-66b9f1036e8c"),
                OwnerId = Guid.Parse("c849aa5a-9a79-4d7a-84f3-e6835f05d242"),
                Name = "User",
                CreateDate = null,
                UpdateDate = null
            },
            new ResourceEntity()
            {
                Id = Guid.Parse("fe216bfd-65af-404a-b30e-60465fca02ee"),
                OwnerId = Guid.Parse("c849aa5a-9a79-4d7a-84f3-e6835f05d242"),
                Name = "Role",
                CreateDate = null,
                UpdateDate = null
            },
            new ResourceEntity()
            {
                Id = Guid.Parse("b8c49fab-8e40-431d-8edc-3a2e17eeb2e6"),
                OwnerId = Guid.Parse("c849aa5a-9a79-4d7a-84f3-e6835f05d242"),
                Name = "Permission",
                CreateDate = null,
                UpdateDate = null
            },
            new ResourceEntity()
            {
                Id = Guid.Parse("5201d4b1-d6bf-488b-b925-7a5f8d1c8a0d"),
                OwnerId = Guid.Parse("a3e10874-01a0-4bb5-80a0-a647e31126ca"),
                Name = "Brand",
                CreateDate = null,
                UpdateDate = null
            },
            new ResourceEntity()
            {
                Id = Guid.Parse("b4c8a3dc-ca22-4972-a4bc-ac6899936231"),
                OwnerId = Guid.Parse("a3e10874-01a0-4bb5-80a0-a647e31126ca"),
                Name = "Supplier",
                CreateDate = null,
                UpdateDate = null
            },
            new ResourceEntity()
            {
                Id = Guid.Parse("9120cce8-d123-4181-8a53-baaa7774599b"),
                OwnerId = Guid.Parse("a3e10874-01a0-4bb5-80a0-a647e31126ca"),
                Name = "Product",
                CreateDate = null,
                UpdateDate = null
            },
        ];
    }
}