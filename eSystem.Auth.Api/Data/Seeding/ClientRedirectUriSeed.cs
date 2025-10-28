using eSystem.Core.Data.Seeding;

namespace eSystem.Auth.Api.Data.Seeding;

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
                RedirectUri = "http://localhost:5501/sso/callback",
                CreateDate = DateTimeOffset.UtcNow,
            }
        ];
    }
}