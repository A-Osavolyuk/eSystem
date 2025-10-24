using eSystem.Domain.Abstraction.Data.Seeding;
using eSystem.Domain.Security.Verification;

namespace eSystem.Auth.Api.Data.Seeding;

public class UserVerificationMethodSeed : Seed<UserVerificationMethodEntity>
{
    public override List<UserVerificationMethodEntity> Get()
    {
        return
        [
            new UserVerificationMethodEntity()
            {
                UserId = Guid.Parse("188c7286-b0b9-4cb1-8f7f-503c6349bc65"),
                Method = VerificationMethod.Email,
                Preferred = true,
                CreateDate = DateTimeOffset.UtcNow,
            }
        ];
    }
}