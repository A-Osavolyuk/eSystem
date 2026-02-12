using eSecurity.Server.Data.Entities;
using eSystem.Core.Data.Seeding;

namespace eSecurity.Server.Data.Seeding;

public class ClientGrantTypeSeed : Seed<ClientGrantTypeEntity>
{
    public override List<ClientGrantTypeEntity> Get()
    {
        return
        [
            // eSecurity
            new ClientGrantTypeEntity
            {
                ClientId = Guid.Parse("392e390f-33bd-4f30-af70-ccbe04bbb2c4"),
                GrantId = Guid.Parse("fd4073e5-1bc7-41ec-8c50-3ca8f0886314"),
            },
            new ClientGrantTypeEntity
            {
                ClientId = Guid.Parse("392e390f-33bd-4f30-af70-ccbe04bbb2c4"),
                GrantId = Guid.Parse("9e90c561-4fae-40c4-b0f1-8c521131d243"),
            },
            
            // eCinema
            new ClientGrantTypeEntity
            {
                ClientId = Guid.Parse("307268b0-005c-4ee4-a0e8-a93bd0010382"),
                GrantId = Guid.Parse("fd4073e5-1bc7-41ec-8c50-3ca8f0886314"),
            },
            new ClientGrantTypeEntity
            {
                ClientId = Guid.Parse("307268b0-005c-4ee4-a0e8-a93bd0010382"),
                GrantId = Guid.Parse("9e90c561-4fae-40c4-b0f1-8c521131d243"),
            },
            
            // eCinema TV App
            new ClientGrantTypeEntity
            {
                ClientId = Guid.Parse("19362a03-8793-4300-82ad-28719f21a8e2"),
                GrantId = Guid.Parse("fd4073e5-1bc7-41ec-8c50-3ca8f0886314"),
            },
            new ClientGrantTypeEntity
            {
                ClientId = Guid.Parse("19362a03-8793-4300-82ad-28719f21a8e2"),
                GrantId = Guid.Parse("0c88259a-f119-4fff-8680-76b8935d7920"),
            },
            
            // eMessage
            new ClientGrantTypeEntity
            {
                ClientId = Guid.Parse("fc1c1662-cd80-4fab-b924-a39168765558"),
                GrantId = Guid.Parse("0ca85e15-9656-46d1-8679-121f18392acb"),
            },
        ];
    }
}