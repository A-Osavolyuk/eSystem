using eSystem.Auth.Api.Data.Entities;
using eSystem.Core.Data.Seeding;
using eSystem.Core.Security.Authentication.ODIC.Token;

namespace eSystem.Auth.Api.Data.Seeding;

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
                CreateDate = DateTimeOffset.UtcNow
            },
            new ClientGrantTypeEntity()
            {
                Id = Guid.CreateVersion7(),
                ClientId = Guid.Parse("392e390f-33bd-4f30-af70-ccbe04bbb2c4"),
                Type = GrantTypes.AuthorizationCode,
                CreateDate = DateTimeOffset.UtcNow
            },
            new ClientGrantTypeEntity()
            {
                Id = Guid.CreateVersion7(),
                ClientId = Guid.Parse("392e390f-33bd-4f30-af70-ccbe04bbb2c4"),
                Type = GrantTypes.ClientCredentials,
                CreateDate = DateTimeOffset.UtcNow
            },
            new ClientGrantTypeEntity()
            {
                Id = Guid.CreateVersion7(),
                ClientId = Guid.Parse("392e390f-33bd-4f30-af70-ccbe04bbb2c4"),
                Type = GrantTypes.DeviceCode,
                CreateDate = DateTimeOffset.UtcNow
            },
        ];
    }
}