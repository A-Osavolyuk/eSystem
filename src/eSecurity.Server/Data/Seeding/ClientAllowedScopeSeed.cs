using eSecurity.Server.Data.Entities;
using eSystem.Core.Data.Seeding;

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
                ScopeId = Guid.Parse("4f49143c-14b4-435c-aa36-9654a9944f63")
            },
            new ClientAllowedScopeEntity
            {
                Id = Guid.CreateVersion7(),
                ClientId = Guid.Parse("392e390f-33bd-4f30-af70-ccbe04bbb2c4"),
                ScopeId = Guid.Parse("eeb1769f-f45e-43f8-b781-790a80eeb952")
            },
            new ClientAllowedScopeEntity
            {
                Id = Guid.CreateVersion7(),
                ClientId = Guid.Parse("392e390f-33bd-4f30-af70-ccbe04bbb2c4"),
                ScopeId = Guid.Parse("d3bd9531-e4b9-4a6c-9706-c7c3957db095")
            },
            new ClientAllowedScopeEntity
            {
                Id = Guid.CreateVersion7(),
                ClientId = Guid.Parse("392e390f-33bd-4f30-af70-ccbe04bbb2c4"),
                ScopeId = Guid.Parse("f067c039-ee63-44bb-b224-00a5872dfe28")
            },
            new ClientAllowedScopeEntity
            {
                Id = Guid.CreateVersion7(),
                ClientId = Guid.Parse("392e390f-33bd-4f30-af70-ccbe04bbb2c4"),
                ScopeId = Guid.Parse("e36782cf-c927-40c7-97d5-3fcc75fd211f")
            },
            new ClientAllowedScopeEntity
            {
                Id = Guid.CreateVersion7(),
                ClientId = Guid.Parse("392e390f-33bd-4f30-af70-ccbe04bbb2c4"),
                ScopeId = Guid.Parse("33721238-6e31-4cd3-9457-11316dbf5eeb")
            },
            new ClientAllowedScopeEntity
            {
                Id = Guid.CreateVersion7(),
                ClientId = Guid.Parse("392e390f-33bd-4f30-af70-ccbe04bbb2c4"),
                ScopeId = Guid.Parse("ca265929-4c5c-41fd-b509-dacaa5c33586")
            },
            new ClientAllowedScopeEntity
            {
                Id = Guid.CreateVersion7(),
                ClientId = Guid.Parse("392e390f-33bd-4f30-af70-ccbe04bbb2c4"),
                ScopeId = Guid.Parse("74be25d7-c054-4d40-a9b9-a3913c4e1b5e")
            },

            // eCinema
            new ClientAllowedScopeEntity
            {
                Id = Guid.CreateVersion7(),
                ClientId = Guid.Parse("307268b0-005c-4ee4-a0e8-a93bd0010382"),
                ScopeId = Guid.Parse("4f49143c-14b4-435c-aa36-9654a9944f63")
            },
            new ClientAllowedScopeEntity
            {
                Id = Guid.CreateVersion7(),
                ClientId = Guid.Parse("307268b0-005c-4ee4-a0e8-a93bd0010382"),
                ScopeId = Guid.Parse("e36782cf-c927-40c7-97d5-3fcc75fd211f")
            },
            new ClientAllowedScopeEntity
            {
                Id = Guid.CreateVersion7(),
                ClientId = Guid.Parse("307268b0-005c-4ee4-a0e8-a93bd0010382"),
                ScopeId = Guid.Parse("eeb1769f-f45e-43f8-b781-790a80eeb952")
            },
            new ClientAllowedScopeEntity
            {
                Id = Guid.CreateVersion7(),
                ClientId = Guid.Parse("307268b0-005c-4ee4-a0e8-a93bd0010382"),
                ScopeId = Guid.Parse("33721238-6e31-4cd3-9457-11316dbf5eeb")
            },
            new ClientAllowedScopeEntity
            {
                Id = Guid.CreateVersion7(),
                ClientId = Guid.Parse("307268b0-005c-4ee4-a0e8-a93bd0010382"),
                ScopeId = Guid.Parse("d3bd9531-e4b9-4a6c-9706-c7c3957db095")
            },
            new ClientAllowedScopeEntity
            {
                Id = Guid.CreateVersion7(),
                ClientId = Guid.Parse("307268b0-005c-4ee4-a0e8-a93bd0010382"),
                ScopeId = Guid.Parse("f067c039-ee63-44bb-b224-00a5872dfe28")
            },
            new ClientAllowedScopeEntity
            {
                Id = Guid.CreateVersion7(),
                ClientId = Guid.Parse("307268b0-005c-4ee4-a0e8-a93bd0010382"),
                ScopeId = Guid.Parse("ca265929-4c5c-41fd-b509-dacaa5c33586")
            },
            new ClientAllowedScopeEntity
            {
                Id = Guid.CreateVersion7(),
                ClientId = Guid.Parse("307268b0-005c-4ee4-a0e8-a93bd0010382"),
                ScopeId = Guid.Parse("74be25d7-c054-4d40-a9b9-a3913c4e1b5e")
            },
            
            // eCinema TV App
            new ClientAllowedScopeEntity
            {
                Id = Guid.CreateVersion7(),
                ClientId = Guid.Parse("19362a03-8793-4300-82ad-28719f21a8e2"),
                ScopeId = Guid.Parse("4f49143c-14b4-435c-aa36-9654a9944f63")
            },
            new ClientAllowedScopeEntity
            {
                Id = Guid.CreateVersion7(),
                ClientId = Guid.Parse("19362a03-8793-4300-82ad-28719f21a8e2"),
                ScopeId = Guid.Parse("eeb1769f-f45e-43f8-b781-790a80eeb952")
            },
            
            // eMessage
            new ClientAllowedScopeEntity
            {
                Id = Guid.CreateVersion7(),
                ClientId = Guid.Parse("fc1c1662-cd80-4fab-b924-a39168765558"),
                ScopeId = Guid.Parse("33721238-6e31-4cd3-9457-11316dbf5eeb")
            },
            new ClientAllowedScopeEntity
            {
                Id = Guid.CreateVersion7(),
                ClientId = Guid.Parse("fc1c1662-cd80-4fab-b924-a39168765558"),
                ScopeId = Guid.Parse("d3bd9531-e4b9-4a6c-9706-c7c3957db095")
            },
            new ClientAllowedScopeEntity
            {
                Id = Guid.CreateVersion7(),
                ClientId = Guid.Parse("fc1c1662-cd80-4fab-b924-a39168765558"),
                ScopeId = Guid.Parse("f067c039-ee63-44bb-b224-00a5872dfe28")
            },
            new ClientAllowedScopeEntity
            {
                Id = Guid.CreateVersion7(),
                ClientId = Guid.Parse("fc1c1662-cd80-4fab-b924-a39168765558"),
                ScopeId = Guid.Parse("eeb1769f-f45e-43f8-b781-790a80eeb952")
            },
            new ClientAllowedScopeEntity
            {
                Id = Guid.CreateVersion7(),
                ClientId = Guid.Parse("fc1c1662-cd80-4fab-b924-a39168765558"),
                ScopeId = Guid.Parse("e36782cf-c927-40c7-97d5-3fcc75fd211f")
            },
            new ClientAllowedScopeEntity
            {
                Id = Guid.CreateVersion7(),
                ClientId = Guid.Parse("fc1c1662-cd80-4fab-b924-a39168765558"),
                ScopeId = Guid.Parse("ca265929-4c5c-41fd-b509-dacaa5c33586")
            },
            new ClientAllowedScopeEntity
            {
                Id = Guid.CreateVersion7(),
                ClientId = Guid.Parse("fc1c1662-cd80-4fab-b924-a39168765558"),
                ScopeId = Guid.Parse("74be25d7-c054-4d40-a9b9-a3913c4e1b5e")
            },
        ];
    }
}