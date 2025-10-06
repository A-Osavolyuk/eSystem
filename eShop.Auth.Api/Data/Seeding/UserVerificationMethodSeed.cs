using eShop.Domain.Abstraction.Data.Seeding;

namespace eShop.Auth.Api.Data.Seeding;

public class UserVerificationMethodSeed : Seed<UserVerificationMethodEntity>
{
    public override List<UserVerificationMethodEntity> Get()
    {
        return
        [
            new UserVerificationMethodEntity()
            {
                UserId = Guid.Parse("abb9d2ed-c3d2-4df9-ba88-eab018b95bc3"),
                MethodId = Guid.Parse("c8ba7087-5582-4a21-b878-1093188dd34f"),
                IsPrimary = true,
                CreateDate = DateTimeOffset.UtcNow,
            }
        ];
    }
}