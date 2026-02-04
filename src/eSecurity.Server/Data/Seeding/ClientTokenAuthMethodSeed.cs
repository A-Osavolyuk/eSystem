using eSecurity.Server.Data.Entities;
using eSecurity.Server.Security.Authorization.Constants;
using eSystem.Core.Data.Seeding;

namespace eSecurity.Server.Data.Seeding;

public class ClientTokenAuthMethodSeed : Seed<ClientTokenAuthMethodEntity>
{
    public override List<ClientTokenAuthMethodEntity> Get()
    {
        return
        [
            // eSecurity
            new ClientTokenAuthMethodEntity()
            {
                Id = Guid.CreateVersion7(),
                ClientId = Guid.Parse("392e390f-33bd-4f30-af70-ccbe04bbb2c4"),
                Method = TokenAuthMethods.ClientSecretBasic
            },
            new ClientTokenAuthMethodEntity()
            {
                Id = Guid.CreateVersion7(),
                ClientId = Guid.Parse("392e390f-33bd-4f30-af70-ccbe04bbb2c4"),
                Method = TokenAuthMethods.ClientSecretPost
            },
            
            // eCinema
            new ClientTokenAuthMethodEntity()
            {
                Id = Guid.CreateVersion7(),
                ClientId = Guid.Parse("307268b0-005c-4ee4-a0e8-a93bd0010382"),
                Method = TokenAuthMethods.ClientSecretBasic
            },
            new ClientTokenAuthMethodEntity()
            {
                Id = Guid.CreateVersion7(),
                ClientId = Guid.Parse("307268b0-005c-4ee4-a0e8-a93bd0010382"),
                Method = TokenAuthMethods.ClientSecretPost
            },
            
            // eCinema TV App
            new ClientTokenAuthMethodEntity()
            {
                Id = Guid.CreateVersion7(),
                ClientId = Guid.Parse("19362a03-8793-4300-82ad-28719f21a8e2"),
                Method = TokenAuthMethods.ClientSecretBasic
            },
            new ClientTokenAuthMethodEntity()
            {
                Id = Guid.CreateVersion7(),
                ClientId = Guid.Parse("19362a03-8793-4300-82ad-28719f21a8e2"),
                Method = TokenAuthMethods.ClientSecretPost
            },
            
            // eMessage
            new ClientTokenAuthMethodEntity()
            {
                Id = Guid.CreateVersion7(),
                ClientId = Guid.Parse("fc1c1662-cd80-4fab-b924-a39168765558"),
                Method = TokenAuthMethods.ClientSecretBasic
            },
            new ClientTokenAuthMethodEntity()
            {
                Id = Guid.CreateVersion7(),
                ClientId = Guid.Parse("fc1c1662-cd80-4fab-b924-a39168765558"),
                Method = TokenAuthMethods.ClientSecretPost
            },
        ];
    }
}