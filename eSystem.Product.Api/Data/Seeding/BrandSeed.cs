using eSystem.Domain.Abstraction.Data.Seeding;
using eSystem.Product.Api.Entities;

namespace eSystem.Product.Api.Data.Seeding;

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
                LogoUrl = "https://upload.wikimedia.org/wikipedia/commons/2/2b/Chiquita_logo_2019.svg",
                CreateDate = DateTimeOffset.UtcNow
            },
            new BrandEntity
            {
                Id = Guid.NewGuid(),
                Name = "Dole",
                Description = "Global leader in fresh fruits and vegetables",
                WebsiteUrl = "https://www.dole.com",
                Country = "USA",
                LogoUrl = "https://upload.wikimedia.org/wikipedia/en/d/d5/Dole_Foods_Logo_Green_Leaf.svg",
                CreateDate = DateTimeOffset.UtcNow
            },
            new BrandEntity
            {
                Id = Guid.NewGuid(),
                Name = "Del Monte",
                Description = "Fresh and packaged fruits and vegetables brand",
                WebsiteUrl = "https://www.delmonte.com",
                Country = "USA",
                LogoUrl = "https://www.logo.wine/a/logo/Del_Monte_Foods/Del_Monte_Foods-Logo.wine.svg",
                CreateDate = DateTimeOffset.UtcNow
            },
            new BrandEntity
            {
                Id = Guid.NewGuid(),
                Name = "Driscoll's",
                Description = "Specializes in berries like strawberries, raspberries, blueberries",
                WebsiteUrl = "https://www.driscolls.com",
                Country = "USA",
                LogoUrl = "https://wp.logos-download.com/wp-content/uploads/2022/01/Driscolls_Logo-700x194.png",
                CreateDate = DateTimeOffset.UtcNow
            },
            new BrandEntity
            {
                Id = Guid.NewGuid(),
                Name = "Fyffes",
                Description = "European brand known for bananas and melons",
                WebsiteUrl = "https://www.fyffes.com",
                Country = "Ireland",
                LogoUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/0/0c/Fyffes_SVG_logo.svg/2560px-Fyffes_SVG_logo.svg.png",
                CreateDate = DateTimeOffset.UtcNow
            }
        ];
    }
}