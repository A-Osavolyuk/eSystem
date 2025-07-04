namespace eShop.Product.Api.Data.Seed;

public class SupplierSeed : Seed<SupplierEntity>
{
    public override List<SupplierEntity> Get()
    {
        return
        [
            new SupplierEntity
            {
                Id = Guid.NewGuid(),
                Name = "Global Fresh Farms",
                Email = "contact@globalfreshfarms.com",
                PhoneNumber = "+1-555-123-4567",
                WebsiteUrl = "https://globalfarms.com/",
                Address = "123 Farm Lane, California, USA",
                Description = "Supplier of fresh organic fruits and vegetables."
            },
            new SupplierEntity
            {
                Id = Guid.NewGuid(),
                Name = "Green Valley Distributors",
                Email = "sales@greenvalleydist.com",
                PhoneNumber = "+44-20-7946-0123",
                WebsiteUrl = "https://www.greenvalleyintl.com/",
                Address = "45 Distribution St, London, UK",
                Description = "Distributor of imported fruits and vegetables in Europe."
            },
            new SupplierEntity
            {
                Id = Guid.NewGuid(),
                Name = "Sunrise Agro Supply",
                Email = "info@sunriseagro.com",
                PhoneNumber = "+91-80-1234-5678",
                WebsiteUrl = "https://sunrise-agro.com/",
                Address = "78 Agro Park, Bangalore, India",
                Description = "Supplier of fresh berries and organic vegetables."
            },
            new SupplierEntity
            {
                Id = Guid.NewGuid(),
                Name = "Tropical Fruit Imports Ltd.",
                Email = "contact@tropicalfruitimports.com",
                PhoneNumber = "+61-2-9876-5432",
                WebsiteUrl = "https://tropifruit.co.uk/",
                Address = "9 Harbor Road, Sydney, Australia",
                Description = "Importer and supplier of tropical fruits."
            },
            new SupplierEntity
            {
                Id = Guid.NewGuid(),
                Name = "Fresh Roots Cooperative",
                Email = "support@freshrootscoop.org",
                PhoneNumber = "+1-604-555-7890",
                WebsiteUrl = "https://freshroots.ca/",
                Address = "456 Farm Road, Vancouver, Canada",
                Description = "Cooperative of local farms supplying seasonal produce."
            }
        ];
    }
}