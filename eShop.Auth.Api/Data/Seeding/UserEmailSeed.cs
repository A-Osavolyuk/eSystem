using eShop.Domain.Abstraction.Data.Seeding;

namespace eShop.Auth.Api.Data.Seeding;

public class UserEmailSeed : Seed<UserEmailEntity>
{
    public override List<UserEmailEntity> Get()
    {
        return
        [
            new UserEmailEntity()
            {
                Id = Guid.CreateVersion7(),
                UserId = Guid.Parse("abb9d2ed-c3d2-4df9-ba88-eab018b95bc3"),
                Email = "sasha.osavolll111@gmail.com",
                NormalizedEmail = "sasha.osavolll111@gmail.com".ToUpperInvariant(),
                IsPrimary = true,
                IsVerified = true,
                CreateDate = DateTimeOffset.UtcNow,
                VerifiedDate = DateTimeOffset.UtcNow,
                PrimaryDate = DateTimeOffset.UtcNow,
            }
        ];
    }
}