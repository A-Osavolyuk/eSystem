using eSecurity.Server.Data.Entities;
using eSystem.Core.Data.Seeding;

namespace eSecurity.Server.Data.Seeding;

public class ClientAllowedScopeSeed : Seed<ClientAllowedScopeEntity>
{
    public override List<ClientAllowedScopeEntity> Get()
    {
        return
        [
            new ClientAllowedScopeEntity()
            {
                ClientId = Guid.Parse("392e390f-33bd-4f30-af70-ccbe04bbb2c4"),
                ScopeId = Guid.Parse("2475899a-e18c-4563-a598-c579113dbda4"),
            },
            new ClientAllowedScopeEntity()
            {
                ClientId = Guid.Parse("392e390f-33bd-4f30-af70-ccbe04bbb2c4"),
                ScopeId = Guid.Parse("7c35e89d-9276-4b3a-9c9a-4b37e9ef137a"),
            },
            new ClientAllowedScopeEntity()
            {
                ClientId = Guid.Parse("392e390f-33bd-4f30-af70-ccbe04bbb2c4"),
                ScopeId = Guid.Parse("26d364e7-be16-4d72-9304-28b141a6e118"),
            },
            new ClientAllowedScopeEntity()
            {
                ClientId = Guid.Parse("392e390f-33bd-4f30-af70-ccbe04bbb2c4"),
                ScopeId = Guid.Parse("4026c2dd-df54-4591-bacf-1b8f446528c5"),
            },
            new ClientAllowedScopeEntity()
            {
                ClientId = Guid.Parse("392e390f-33bd-4f30-af70-ccbe04bbb2c4"),
                ScopeId = Guid.Parse("8d2269e7-ae28-4911-b5ef-6f47418fb65e"),
            },
            new ClientAllowedScopeEntity()
            {
                ClientId = Guid.Parse("392e390f-33bd-4f30-af70-ccbe04bbb2c4"),
                ScopeId = Guid.Parse("865d1609-76c6-4928-a2f0-d1ca77f1498b"),
            }
        ];
    }
}