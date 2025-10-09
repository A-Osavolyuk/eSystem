using eShop.Domain.Abstraction.Data.Seeding;

namespace eShop.Auth.Api.Data.Seeding;

public class LockoutSeed : Seed<UserLockoutStateEntity>
{
    public override List<UserLockoutStateEntity> Get()
    {
        return
        [
            new UserLockoutStateEntity()
            {
                Id = Guid.CreateVersion7(),
                UserId = Guid.Parse("abb9d2ed-c3d2-4df9-ba88-eab018b95bc3"),
                Enabled = false,
                Permanent = false,
                CreateDate = DateTimeOffset.UtcNow,
            }
        ];
    }
}