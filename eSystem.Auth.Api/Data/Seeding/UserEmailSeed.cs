using eSystem.Core.Data.Seeding;
using eSystem.Core.Security.Identity.Email;

namespace eSystem.Auth.Api.Data.Seeding;

public class UserEmailSeed : Seed<UserEmailEntity>
{
    public override List<UserEmailEntity> Get()
    {
        return
        [
            new UserEmailEntity()
            {
                Id = Guid.CreateVersion7(),
                UserId = Guid.Parse("188c7286-b0b9-4cb1-8f7f-503c6349bc65"),
                Email = "sasha.osavolll111@gmail.com",
                NormalizedEmail = "sasha.osavolll111@gmail.com".ToUpperInvariant(),
                Type = EmailType.Primary,
                IsVerified = true,
                CreateDate = DateTimeOffset.UtcNow,
                VerifiedDate = DateTimeOffset.UtcNow,
            }
        ];
    }
}