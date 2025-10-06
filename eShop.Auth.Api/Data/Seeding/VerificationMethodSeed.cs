using eShop.Domain.Abstraction.Data.Seeding;

namespace eShop.Auth.Api.Data.Seeding;

public class VerificationMethodSeed : Seed<VerificationMethodEntity>
{
    public override List<VerificationMethodEntity> Get()
    {
        return
        [
            new VerificationMethodEntity()
            {
                Id = Guid.Parse("c8ba7087-5582-4a21-b878-1093188dd34f"),
                Method = VerificationMethod.Email,
                CreateDate = DateTimeOffset.UtcNow
            },
            new VerificationMethodEntity()
            {
                Id = Guid.Parse("7067c792-6f13-4c97-aa96-c590dc9569ed"),
                Method = VerificationMethod.Passkey,
                CreateDate = DateTimeOffset.UtcNow
            },
            new VerificationMethodEntity()
            {
                Id = Guid.Parse("470c2478-c55a-4f33-be9f-01e93aa36719"),
                Method = VerificationMethod.AuthenticatorApp,
                CreateDate = DateTimeOffset.UtcNow
            }
        ];
    }
}