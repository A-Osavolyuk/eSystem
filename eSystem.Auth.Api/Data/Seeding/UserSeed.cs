using eSystem.Auth.Api.Data.Entities;
using eSystem.Core.Data.Seeding;

namespace eSystem.Auth.Api.Data.Seeding;

public class UserSeed : Seed<UserEntity>
{
    public override List<UserEntity> Get()
    {
        return
        [
            new ()
            {
                Id = Guid.Parse("188c7286-b0b9-4cb1-8f7f-503c6349bc65"),
                Username = "pipidastr",
                NormalizedUsername = "PIPIDASTR".ToUpper(),
                PasswordHash = "ARAnAAAl1FoQrHhNWGK51c8k0FFv1BuyOTZvNXrRWI7EVVDW5ScOzlWykcg+O8MKwnwzJEs=",
                AccountConfirmed = true,
            }
        ];
    }
}