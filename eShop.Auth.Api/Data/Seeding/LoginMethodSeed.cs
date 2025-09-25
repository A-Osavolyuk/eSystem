using eShop.Domain.Abstraction.Data.Seeding;

namespace eShop.Auth.Api.Data.Seeding;

public class LoginMethodSeed : Seed<LoginMethodEntity>
{
    public override List<LoginMethodEntity> Get()
    {
        return
        [
            new LoginMethodEntity()
            {
                Id = Guid.CreateVersion7(),
                Type = LoginType.Password,
                CreateDate = DateTimeOffset.UtcNow
            },
            new LoginMethodEntity()
            {
                Id = Guid.CreateVersion7(),
                Type = LoginType.TwoFactor,
                CreateDate = DateTimeOffset.UtcNow
            },
            new LoginMethodEntity()
            {
                Id = Guid.CreateVersion7(),
                Type = LoginType.OAuth,
                CreateDate = DateTimeOffset.UtcNow
            },
            new LoginMethodEntity()
            {
                Id = Guid.CreateVersion7(),
                Type = LoginType.Passkey,
                CreateDate = DateTimeOffset.UtcNow
            }
        ];
    }
}