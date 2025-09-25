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
                Id = Guid.Parse("33bed4d7-0976-451b-b386-c1321821bde1"),
                Type = LoginType.Password,
                CreateDate = DateTimeOffset.UtcNow
            },
            new LoginMethodEntity()
            {
                Id = Guid.Parse("bda1bd08-ec94-4e36-bd23-541f70649118"),
                Type = LoginType.TwoFactor,
                CreateDate = DateTimeOffset.UtcNow
            },
            new LoginMethodEntity()
            {
                Id = Guid.Parse("497c4c1d-680f-4322-9cf6-008f3b2da475"),
                Type = LoginType.OAuth,
                CreateDate = DateTimeOffset.UtcNow
            },
            new LoginMethodEntity()
            {
                Id = Guid.Parse("4565fc62-c867-40ae-b24b-d833c8bbf590"),
                Type = LoginType.Passkey,
                CreateDate = DateTimeOffset.UtcNow
            }
        ];
    }
}