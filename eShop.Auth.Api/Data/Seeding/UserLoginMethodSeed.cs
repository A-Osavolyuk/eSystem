using eShop.Domain.Abstraction.Data.Seeding;

namespace eShop.Auth.Api.Data.Seeding;

public class UserLoginMethodSeed : Seed<UserLoginMethodEntity>
{
    public override List<UserLoginMethodEntity> Get()
    {
        return
        [
            new UserLoginMethodEntity()
            {
                UserId = Guid.Parse("abb9d2ed-c3d2-4df9-ba88-eab018b95bc3"),
                MethodId = Guid.Parse("33bed4d7-0976-451b-b386-c1321821bde1"),
                CreateDate = DateTimeOffset.UtcNow
            }
        ];
    }
}