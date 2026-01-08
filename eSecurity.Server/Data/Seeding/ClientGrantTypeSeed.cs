using eSecurity.Server.Data.Entities;
using eSystem.Core.Data.Seeding;
using eSystem.Core.Security.Authentication.Oidc.Token;

namespace eSecurity.Server.Data.Seeding;

public class ClientGrantTypeSeed : Seed<ClientGrantTypeEntity>
{
    public override List<ClientGrantTypeEntity> Get()
    {
        return
        [
            new ClientGrantTypeEntity()
            {
                Id = Guid.CreateVersion7(),
                ClientId = Guid.Parse("392e390f-33bd-4f30-af70-ccbe04bbb2c4"),
                Type = GrantTypes.RefreshToken,
            },
            new ClientGrantTypeEntity()
            {
                Id = Guid.CreateVersion7(),
                ClientId = Guid.Parse("392e390f-33bd-4f30-af70-ccbe04bbb2c4"),
                Type = GrantTypes.AuthorizationCode,
            },
            new ClientGrantTypeEntity()
            {
                Id = Guid.CreateVersion7(),
                ClientId = Guid.Parse("392e390f-33bd-4f30-af70-ccbe04bbb2c4"),
                Type = GrantTypes.ClientCredentials,
            },
            new ClientGrantTypeEntity()
            {
                Id = Guid.CreateVersion7(),
                ClientId = Guid.Parse("392e390f-33bd-4f30-af70-ccbe04bbb2c4"),
                Type = GrantTypes.DeviceCode,
            },
        ];
    }
}