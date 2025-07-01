namespace eShop.Product.Api.Data.Seed;

public class CategorySeed : Seed<CategoryEntity>
{
    public override List<CategoryEntity> Get()
    {
        return
        [
            new CategoryEntity()
            {
                Id = Guid.Parse("b08c35a4-fc66-43f7-a98f-35e51d149d29"),
                Name = "Food",
                CreateDate = DateTimeOffset.UtcNow
            },
            new CategoryEntity()
            {
                Id = Guid.Parse("2e871483-9f46-457d-bcd7-3706d4c91b62"),
                Name = "Electronics",
                CreateDate = DateTimeOffset.UtcNow
            },
            new CategoryEntity()
            {
                Id = Guid.Parse("c33b46ae-4e57-4173-b6d2-0c6a9ff842f0"),
                Name = "Clothing",
                CreateDate = DateTimeOffset.UtcNow
            },
            new CategoryEntity()
            {
                Id = Guid.Parse("d6d1a0f0-95db-4fa6-871c-1f22e4f74945"),
                Name = "Shoes",
                CreateDate = DateTimeOffset.UtcNow
            },
        ];
    }
}