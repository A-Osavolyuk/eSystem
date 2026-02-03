using eSecurity.Server.Data.Entities;
using eSystem.Core.Data.Seeding;
using eSystem.Core.Security.Authentication.OpenIdConnect.Constants;

namespace eSecurity.Server.Data.Seeding;

public class ClientAllowedScopeSeed : Seed<ClientAllowedScopeEntity>
{
    public override List<ClientAllowedScopeEntity> Get()
    {
        return
        [
            // eSecurity.Client
            new ClientAllowedScopeEntity
            {
                Id = Guid.CreateVersion7(),
                ClientId = Guid.Parse("392e390f-33bd-4f30-af70-ccbe04bbb2c4"),
                Scope = ScopeTypes.OpenId
            },
            new ClientAllowedScopeEntity
            {
                Id = Guid.CreateVersion7(),
                ClientId = Guid.Parse("392e390f-33bd-4f30-af70-ccbe04bbb2c4"),
                Scope = ScopeTypes.OfflineAccess
            },
            new ClientAllowedScopeEntity
            {
                Id = Guid.CreateVersion7(),
                ClientId = Guid.Parse("392e390f-33bd-4f30-af70-ccbe04bbb2c4"),
                Scope = ScopeTypes.Email
            },
            new ClientAllowedScopeEntity
            {
                Id = Guid.CreateVersion7(),
                ClientId = Guid.Parse("392e390f-33bd-4f30-af70-ccbe04bbb2c4"),
                Scope = ScopeTypes.Phone
            },
            new ClientAllowedScopeEntity
            {
                Id = Guid.CreateVersion7(),
                ClientId = Guid.Parse("392e390f-33bd-4f30-af70-ccbe04bbb2c4"),
                Scope = ScopeTypes.Address
            },
            new ClientAllowedScopeEntity
            {
                Id = Guid.CreateVersion7(),
                ClientId = Guid.Parse("392e390f-33bd-4f30-af70-ccbe04bbb2c4"),
                Scope = ScopeTypes.Profile
            },

            // eCinema.Client
            new ClientAllowedScopeEntity
            {
                Id = Guid.CreateVersion7(),
                ClientId = Guid.Parse("307268b0-005c-4ee4-a0e8-a93bd0010382"),
                Scope = ScopeTypes.OpenId,
            },
            new ClientAllowedScopeEntity
            {
                Id = Guid.CreateVersion7(),
                ClientId = Guid.Parse("307268b0-005c-4ee4-a0e8-a93bd0010382"),
                Scope = ScopeTypes.Address,
            },
            new ClientAllowedScopeEntity
            {
                Id = Guid.CreateVersion7(),
                ClientId = Guid.Parse("307268b0-005c-4ee4-a0e8-a93bd0010382"),
                Scope = ScopeTypes.OfflineAccess,
            },
            new ClientAllowedScopeEntity
            {
                Id = Guid.CreateVersion7(),
                ClientId = Guid.Parse("307268b0-005c-4ee4-a0e8-a93bd0010382"),
                Scope = ScopeTypes.Profile,
            },
            new ClientAllowedScopeEntity
            {
                Id = Guid.CreateVersion7(),
                ClientId = Guid.Parse("307268b0-005c-4ee4-a0e8-a93bd0010382"),
                Scope = ScopeTypes.Email,
            },
            new ClientAllowedScopeEntity
            {
                Id = Guid.CreateVersion7(),
                ClientId = Guid.Parse("307268b0-005c-4ee4-a0e8-a93bd0010382"),
                Scope = ScopeTypes.Phone
            },
            
            // eMessage
            new ClientAllowedScopeEntity
            {
                Id = Guid.CreateVersion7(),
                ClientId = Guid.Parse("fc1c1662-cd80-4fab-b924-a39168765558"),
                Scope = ScopeTypes.Profile,
            },
            new ClientAllowedScopeEntity
            {
                Id = Guid.CreateVersion7(),
                ClientId = Guid.Parse("fc1c1662-cd80-4fab-b924-a39168765558"),
                Scope = ScopeTypes.Email,
            },
            new ClientAllowedScopeEntity
            {
                Id = Guid.CreateVersion7(),
                ClientId = Guid.Parse("fc1c1662-cd80-4fab-b924-a39168765558"),
                Scope = ScopeTypes.Phone
            },
            new ClientAllowedScopeEntity
            {
                Id = Guid.CreateVersion7(),
                ClientId = Guid.Parse("fc1c1662-cd80-4fab-b924-a39168765558"),
                Scope = ScopeTypes.OfflineAccess,
            },
            new ClientAllowedScopeEntity
            {
                Id = Guid.CreateVersion7(),
                ClientId = Guid.Parse("fc1c1662-cd80-4fab-b924-a39168765558"),
                Scope = ScopeTypes.Address,
            }
        ];
    }
}