namespace eShop.Auth.Api.Data.Seed;

public class ResourceSeed : Seed<ResourceEntity, Guid>
{
    public override List<ResourceEntity> Get()
    {
        return
        [
            new ResourceEntity()
            {
                Id = Guid.Parse("1a06eabb-3354-4b03-be52-ab42743eaa97"),
                Name = "account",
                CreateDate = null,
                UpdateDate = null
            },
            new ResourceEntity()
            {
                Id = Guid.Parse("70969390-c949-4426-b3c1-7fd86580b62e"),
                Name = "admin",
                CreateDate = null,
                UpdateDate = null
            },
        ];
    }
}