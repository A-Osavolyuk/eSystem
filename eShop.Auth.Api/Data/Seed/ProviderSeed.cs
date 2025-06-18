namespace eShop.Auth.Api.Data.Seed;

public class ProviderSeed : Seed<ProviderEntity>
{
    public override List<ProviderEntity> Get()
    {
        return
        [
            new ProviderEntity()
            {
                Id = Guid.Parse("a4d155e3-1746-4f4d-af0e-e5ebca3ffcac"),
                Name = "Email",
                CreateDate = DateTime.UtcNow,
                UpdateDate = null
            },
            new ProviderEntity()
            {
                Id = Guid.Parse("60c95fd9-c0e1-4a08-8d21-50c66f3a91d4"),
                Name = "Sms",
                CreateDate = DateTime.UtcNow,
                UpdateDate = null
            },
            new ProviderEntity()
            {
                Id = Guid.Parse("f01b5a5a-8f4b-4814-8ade-2935e02af0a5"),
                Name = "Authenticator",
                CreateDate = DateTime.UtcNow,
                UpdateDate = null
            }
        ];
    }
}