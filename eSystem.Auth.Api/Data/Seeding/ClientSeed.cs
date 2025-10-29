using eSystem.Auth.Api.Data.Entities;
using eSystem.Core.Data.Seeding;
using eSystem.Core.Security.Authentication.SSO.Client;

namespace eSystem.Auth.Api.Data.Seeding;

public class ClientSeed : Seed<ClientEntity>
{
    public override List<ClientEntity> Get()
    {
        return
        [
            new ClientEntity()
            {
                Id = Guid.Parse("392e390f-33bd-4f30-af70-ccbe04bbb2c4"),
                ClientId = "eAccount",
                ClientSecret = "2f213a036e325a55dc19320f03c2fad7c13f0169788b5968686cb4931341c393a651d7e6",
                Name = "eAccount",
                RequireClientSecret = true,
                RequirePkce = false,
                AllowOfflineAccess = true,
                Type = ClientType.Confidential,
                CreateDate = DateTimeOffset.UtcNow
            }
        ];
    }
}