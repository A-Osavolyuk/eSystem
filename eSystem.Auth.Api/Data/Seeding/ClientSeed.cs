using eSystem.Auth.Api.Security.Authentication.SSO;
using eSystem.Core.Data.Seeding;

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
                ClientId = "client-sso",
                ClientSecret = "2f213a036e325a55dc19320f03c2fad7c13f0169788b5968686cb4931341c393a651d7e6",
                Name = "SSO Client",
                RequireClientSecret = true,
                RequirePkce = false,
                AllowOfflineAccess = true,
                Type = ClientType.Confidential,
                CreateDate = DateTimeOffset.UtcNow
            }
        ];
    }
}