using eShop.Domain.Abstraction.Data.Seeding;

namespace eShop.Auth.Api.Data.Seeding;

public class UserTwoFactorProviderSeed : Seed<UserTwoFactorProviderEntity>
{
    public override List<UserTwoFactorProviderEntity> Get()
    {
        return
        [
            new UserTwoFactorProviderEntity()
            {
                UserId = Guid.Parse("abb9d2ed-c3d2-4df9-ba88-eab018b95bc3"),
                ProviderId = Guid.Parse("a4d155e3-1746-4f4d-af0e-e5ebca3ffcac"),
                CreateDate = DateTimeOffset.UtcNow,
                Subscribed = false
            },
            new UserTwoFactorProviderEntity()
            {
                UserId = Guid.Parse("abb9d2ed-c3d2-4df9-ba88-eab018b95bc3"),
                ProviderId = Guid.Parse("60c95fd9-c0e1-4a08-8d21-50c66f3a91d4"),
                CreateDate = DateTimeOffset.UtcNow,
                Subscribed = false
            },
            new UserTwoFactorProviderEntity()
            {
                UserId = Guid.Parse("abb9d2ed-c3d2-4df9-ba88-eab018b95bc3"),
                ProviderId = Guid.Parse("f01b5a5a-8f4b-4814-8ade-2935e02af0a5"),
                CreateDate = DateTimeOffset.UtcNow,
                Subscribed = false
            },
        ];
    }
}