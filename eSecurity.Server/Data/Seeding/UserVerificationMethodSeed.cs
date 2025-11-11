using eSecurity.Core.Security.Authorization.Access;
using eSecurity.Server.Data.Entities;
using eSystem.Core.Data.Seeding;

namespace eSecurity.Server.Data.Seeding;

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