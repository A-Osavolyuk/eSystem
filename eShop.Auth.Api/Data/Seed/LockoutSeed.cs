using eShop.Domain.Abstraction.Data.Seeding;

namespace eShop.Auth.Api.Data.Seed;

public class LockoutSeed : Seed<LockoutStateEntity>
{
    public override List<LockoutStateEntity> Get()
    {
        return
        [
            new LockoutStateEntity()
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