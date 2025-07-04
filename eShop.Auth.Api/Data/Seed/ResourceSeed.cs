namespace eShop.Auth.Api.Data.Seed;

public class ResourceSeed : Seed<ResourceEntity>
{
    public override List<ResourceEntity> Get()
    {
        return
        [
            new ResourceEntity()
            {
                Id = Guid.Parse("1a06eabb-3354-4b03-be52-ab42743eaa97"),
                Name = "Account",
                CreateDate = null,
                UpdateDate = null
            },
            new ResourceEntity()
            {
                Id = Guid.Parse("e3fef8c4-0105-419a-b889-66b9f1036e8c"),
                Name = "User",
                CreateDate = null,
                UpdateDate = null
            },
            new ResourceEntity()
            {
                Id = Guid.Parse("fe216bfd-65af-404a-b30e-60465fca02ee"),
                Name = "Role",
                CreateDate = null,
                UpdateDate = null
            },
            new ResourceEntity()
            {
                Id = Guid.Parse("b8c49fab-8e40-431d-8edc-3a2e17eeb2e6"),
                Name = "Permission",
                CreateDate = null,
                UpdateDate = null
            },
            new ResourceEntity()
            {
                Id = Guid.Parse("5201d4b1-d6bf-488b-b925-7a5f8d1c8a0d"),
                Name = "Brand",
                CreateDate = null,
                UpdateDate = null
            },
            new ResourceEntity()
            {
                Id = Guid.Parse("b4c8a3dc-ca22-4972-a4bc-ac6899936231"),
                Name = "Supplier",
                CreateDate = null,
                UpdateDate = null
            },
            new ResourceEntity()
            {
                Id = Guid.Parse("9120cce8-d123-4181-8a53-baaa7774599b"),
                Name = "Product",
                CreateDate = null,
                UpdateDate = null
            },
        ];
    }
}