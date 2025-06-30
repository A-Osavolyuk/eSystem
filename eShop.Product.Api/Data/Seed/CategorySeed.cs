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
            }
        ];
    }
}