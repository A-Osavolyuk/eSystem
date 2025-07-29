using eShop.Domain.Abstraction.Data.Seeding;

namespace eShop.Auth.Api.Data.Seeding;

public class UserSeed : Seed<UserEntity>
{
    public override List<UserEntity> Get()
    {
        return
        [
            new ()
            {
                Id = Guid.Parse("abb9d2ed-c3d2-4df9-ba88-eab018b95bc3"),
                Email = "sasha.osavolll111@gmail.com",
                NormalizedEmail = "sasha.osavolll111@gmail.com".ToUpper(),
                UserName = "pipidastr",
                NormalizedUserName = "PIPIDASTR".ToUpper(),
                PasswordHash = "ARAnAAAl1FoQrHhNWGK51c8k0FFv1BuyOTZvNXrRWI7EVVDW5ScOzlWykcg+O8MKwnwzJEs=",
                EmailConfirmed = true,
                AccountConfirmed = true,
            }
        ];
    }
}