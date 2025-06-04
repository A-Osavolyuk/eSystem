namespace eShop.Auth.Api.Data.Seed;

public class UserProviderSeed : Seed<UserProviderEntity>
{
    public override List<UserProviderEntity> Get()
    {
        return
        [
            new UserProviderEntity()
            {
                UserId = Guid.Parse("abb9d2ed-c3d2-4df9-ba88-eab018b95bc3"),
                ProviderId = Guid.Parse("a4d155e3-1746-4f4d-af0e-e5ebca3ffcac"),
                CreateDate = DateTimeOffset.UtcNow,
                UpdateDate = null
            },
        ];
    }
}