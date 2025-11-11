using eSecurity.Server.Data.Entities;
using eSystem.Core.Data.Seeding;

namespace eSecurity.Server.Data.Seeding;

public class ClientRedirectUriSeed : Seed<ClientRedirectUriEntity>
{
    public override List<ClientRedirectUriEntity> Get()
    {
        return
        [
            new ClientRedirectUriEntity()
            {
                Id = Guid.CreateVersion7(),
                ClientId = Guid.Parse("392e390f-33bd-4f30-af70-ccbe04bbb2c4"),
                Uri = "http://localhost:5501/connect/callback",
                CreateDate = DateTimeOffset.UtcNow,
            }
        ];
    }
}