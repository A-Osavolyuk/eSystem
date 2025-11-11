using eSecurity.Server.Data.Entities;
using eSystem.Core.Data.Seeding;

namespace eSecurity.Server.Data.Seeding;

public class ClientPostLogoutRedirectUriSeed : Seed<ClientPostLogoutRedirectUriEntity>
{
    public override List<ClientPostLogoutRedirectUriEntity> Get()
    {
        return
        [
            new()
            {
                Id = Guid.CreateVersion7(),
                ClientId = Guid.Parse("392e390f-33bd-4f30-af70-ccbe04bbb2c4"),
                Uri = "http://localhost:5501/connect/logout/callback",
                CreateDate = DateTimeOffset.UtcNow,
            }
        ];
    }
}