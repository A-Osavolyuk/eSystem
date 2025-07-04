namespace eShop.Product.Api.Data.Seed;

public class BrandSeed : Seed<BrandEntity>
{
    public override List<BrandEntity> Get()
    {
        return
        [
            new BrandEntity
            {
                Id = Guid.NewGuid(),
                Name = "Chiquita",
                Description = "Well-known brand for bananas and tropical fruits",
                WebsiteUrl = "https://www.chiquita.com",
                Country = "USA",
                LogoUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/7/70/Chiquita_logo.svg/1200px-Chiquita_logo.svg.png",
                CreateDate = DateTimeOffset.UtcNow
            },
            new BrandEntity
            {
                Id = Guid.NewGuid(),
                Name = "Dole",
                Description = "Global leader in fresh fruits and vegetables",
                WebsiteUrl = "https://www.dole.com",
                Country = "USA",
                LogoUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/7/74/Dole_logo.svg/1200px-Dole_logo.svg.png",
                CreateDate = DateTimeOffset.UtcNow
            },
            new BrandEntity
            {
                Id = Guid.NewGuid(),
                Name = "Del Monte",
                Description = "Fresh and packaged fruits and vegetables brand",
                WebsiteUrl = "https://www.delmonte.com",
                Country = "USA",
                LogoUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/2/29/Del_Monte_logo.svg/1200px-Del_Monte_logo.svg.png",
                CreateDate = DateTimeOffset.UtcNow
            },
            new BrandEntity
            {
                Id = Guid.NewGuid(),
                Name = "Driscoll's",
                Description = "Specializes in berries like strawberries, raspberries, blueberries",
                WebsiteUrl = "https://www.driscolls.com",
                Country = "USA",
                LogoUrl = "https://upload.wikimedia.org/wikipedia/en/thumb/2/25/Driscolls_logo.svg/1200px-Driscolls_logo.svg.png",
                CreateDate = DateTimeOffset.UtcNow
            },
            new BrandEntity
            {
                Id = Guid.NewGuid(),
                Name = "Fyffes",
                Description = "European brand known for bananas and melons",
                WebsiteUrl = "https://www.fyffes.com",
                Country = "Ireland",
                LogoUrl = "https://upload.wikimedia.org/wikipedia/en/thumb/1/13/Fyffes_logo.svg/1200px-Fyffes_logo.svg.png",
                CreateDate = DateTimeOffset.UtcNow
            }
        ];
    }
}