using eSystem.Domain.Abstraction.Data.Seeding;

namespace eSystem.Auth.Api.Data.Seeding;

public class LockoutSeed : Seed<UserLockoutStateEntity>
{
    public override List<UserLockoutStateEntity> Get()
    {
        return
        [
            new UserLockoutStateEntity()
            {
                Id = Guid.CreateVersion7(),
                UserId = Guid.Parse("188c7286-b0b9-4cb1-8f7f-503c6349bc65"),
                Permanent = false,
                CreateDate = DateTimeOffset.UtcNow,
            }
        ];
    }
}