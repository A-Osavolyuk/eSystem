using eShop.Domain.Abstraction.Data.Seeding;

namespace eShop.Auth.Api.Data.Seeding;

public class OAuthProviderSeed : Seed<OAuthProviderEntity>
{
    public override List<OAuthProviderEntity> Get()
    {
        return
        [
            new OAuthProviderEntity()
            {
                Id = Guid.CreateVersion7(),
                Name = "Google",
            },
            new OAuthProviderEntity()
            {
                Id = Guid.CreateVersion7(),
                Name = "Microsoft",
            },
            new OAuthProviderEntity()
            {
                Id = Guid.CreateVersion7(),
                Name = "Facebook",
            },
            new OAuthProviderEntity()
            {
                Id = Guid.CreateVersion7(),
                Name = "X",
            }
        ];
    }
}