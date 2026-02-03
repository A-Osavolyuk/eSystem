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
                Scope = ScopesType.OpenId
            },
            new ClientAllowedScopeEntity
            {
                Id = Guid.CreateVersion7(),
                ClientId = Guid.Parse("392e390f-33bd-4f30-af70-ccbe04bbb2c4"),
                Scope = ScopesType.OfflineAccess
            },
            new ClientAllowedScopeEntity
            {
                Id = Guid.CreateVersion7(),
                ClientId = Guid.Parse("392e390f-33bd-4f30-af70-ccbe04bbb2c4"),
                Scope = ScopesType.Email
            },
            new ClientAllowedScopeEntity
            {
                Id = Guid.CreateVersion7(),
                ClientId = Guid.Parse("392e390f-33bd-4f30-af70-ccbe04bbb2c4"),
                Scope = ScopesType.Phone
            },
            new ClientAllowedScopeEntity
            {
                Id = Guid.CreateVersion7(),
                ClientId = Guid.Parse("392e390f-33bd-4f30-af70-ccbe04bbb2c4"),
                Scope = ScopesType.Address
            },
            new ClientAllowedScopeEntity
            {
                Id = Guid.CreateVersion7(),
                ClientId = Guid.Parse("392e390f-33bd-4f30-af70-ccbe04bbb2c4"),
                Scope = ScopesType.Profile
            },

            // eCinema.Client
            new ClientAllowedScopeEntity
            {
                Id = Guid.CreateVersion7(),
                ClientId = Guid.Parse("307268b0-005c-4ee4-a0e8-a93bd0010382"),
                Scope = ScopesType.OpenId,
            },
            new ClientAllowedScopeEntity
            {
                Id = Guid.CreateVersion7(),
                ClientId = Guid.Parse("307268b0-005c-4ee4-a0e8-a93bd0010382"),
                Scope = ScopesType.Address,
            },
            new ClientAllowedScopeEntity
            {
                Id = Guid.CreateVersion7(),
                ClientId = Guid.Parse("307268b0-005c-4ee4-a0e8-a93bd0010382"),
                Scope = ScopesType.OfflineAccess,
            },
            new ClientAllowedScopeEntity
            {
                Id = Guid.CreateVersion7(),
                ClientId = Guid.Parse("307268b0-005c-4ee4-a0e8-a93bd0010382"),
                Scope = ScopesType.Profile,
            },
            new ClientAllowedScopeEntity
            {
                Id = Guid.CreateVersion7(),
                ClientId = Guid.Parse("307268b0-005c-4ee4-a0e8-a93bd0010382"),
                Scope = ScopesType.Email,
            },
            new ClientAllowedScopeEntity
            {
                Id = Guid.CreateVersion7(),
                ClientId = Guid.Parse("307268b0-005c-4ee4-a0e8-a93bd0010382"),
                Scope = ScopesType.Phone
            },
            
            // eMessage
            new ClientAllowedScopeEntity
            {
                Id = Guid.CreateVersion7(),
                ClientId = Guid.Parse("fc1c1662-cd80-4fab-b924-a39168765558"),
                Scope = ScopesType.Profile,
            },
            new ClientAllowedScopeEntity
            {
                Id = Guid.CreateVersion7(),
                ClientId = Guid.Parse("fc1c1662-cd80-4fab-b924-a39168765558"),
                Scope = ScopesType.Email,
            },
            new ClientAllowedScopeEntity
            {
                Id = Guid.CreateVersion7(),
                ClientId = Guid.Parse("fc1c1662-cd80-4fab-b924-a39168765558"),
                Scope = ScopesType.Phone
            },
            new ClientAllowedScopeEntity
            {
                Id = Guid.CreateVersion7(),
                ClientId = Guid.Parse("fc1c1662-cd80-4fab-b924-a39168765558"),
                Scope = ScopesType.OfflineAccess,
            },
            new ClientAllowedScopeEntity
            {
                Id = Guid.CreateVersion7(),
                ClientId = Guid.Parse("fc1c1662-cd80-4fab-b924-a39168765558"),
                Scope = ScopesType.Address,
            }
        ];
    }
}