using eSecurity.Server.Data.Entities;
using eSystem.Core.Data.Seeding;

namespace eSecurity.Server.Data.Seeding;

public class PasswordSeed : Seed<PasswordEntity>
{
    public override List<PasswordEntity> Get()
    {
        return
        [
            new()
            {
                Id = Guid.CreateVersion7(),
                UserId = Guid.Parse("188c7286-b0b9-4cb1-8f7f-503c6349bc65"),
                Hash = "ARAnAAAl1FoQrHhNWGK51c8k0FFv1BuyOTZvNXrRWI7EVVDW5ScOzlWykcg+O8MKwnwzJEs="
            }
        ];
    }
}