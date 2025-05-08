namespace eShop.Auth.Api.Data.Seed;

public class ProviderSeed : Seed<ProviderEntity, Guid>
{
    public override List<ProviderEntity> Get()
    {
        return
        [
            new ProviderEntity()
            {
                Id = Guid.CreateVersion7(),
                Name = "Email",
                CreateDate = DateTime.UtcNow,
                UpdateDate = null
            },
            new ProviderEntity()
            {
                Id = Guid.CreateVersion7(),
                Name = "Sms",
                CreateDate = DateTime.UtcNow,
                UpdateDate = null
            },
            new ProviderEntity()
            {
                Id = Guid.CreateVersion7(),
                Name = "Authenticator",
                CreateDate = DateTime.UtcNow,
                UpdateDate = null
            }
        ];
    }
}