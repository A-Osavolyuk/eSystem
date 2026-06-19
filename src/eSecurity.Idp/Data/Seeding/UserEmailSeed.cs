using eSecurity.Idp.Data.Entities;
using eSecurity.Core.Security.Identity;
using eSecurity.Idp.Common.Validation;
using eSystem.Core.Server.Data.Seeding;

namespace eSecurity.Idp.Data.Seeding;

public class UserEmailSeed : Seed<UserEmailEntity>
{
    public override List<UserEmailEntity> Get()
    {
        return
        [
            new UserEmailEntity
            {
                Id = Guid.CreateVersion7(),
                UserId = Guid.Parse("188c7286-b0b9-4cb1-8f7f-503c6349bc65"),
                Email = "sasha.osavolll111@gmail.com",
                NormalizedEmail = Normalizer.Normalize("sasha.osavolll111@gmail.com"),
                Type = EmailType.Primary,
                IsVerified = true,
                VerifiedAt = DateTimeOffset.UtcNow,
            }
        ];
    }
}