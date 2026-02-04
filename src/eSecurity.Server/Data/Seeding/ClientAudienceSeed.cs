using eSecurity.Server.Data.Entities;
using eSystem.Core.Data.Seeding;

namespace eSecurity.Server.Data.Seeding;

public sealed class ClientAudienceSeed : Seed<ClientAudienceEntity>
{
    public override List<ClientAudienceEntity> Get()
    {
        return
        [
            // eSecurity
            new ClientAudienceEntity()
            {
                Id = Guid.CreateVersion7(),
                ClientId = Guid.Parse("392e390f-33bd-4f30-af70-ccbe04bbb2c4"),
                Audience = "api://esecurity"
            },
            
            // eCinema
            new ClientAudienceEntity()
            {
                Id = Guid.CreateVersion7(),
                ClientId = Guid.Parse("307268b0-005c-4ee4-a0e8-a93bd0010382"),
                Audience = "api://ecinema"
            },
            
            // eCinema TV App
            new ClientAudienceEntity()
            {
                Id = Guid.CreateVersion7(),
                ClientId = Guid.Parse("19362a03-8793-4300-82ad-28719f21a8e2"),
                Audience = "api://ecinema-tv"
            },
            
            // eMessage
            new ClientAudienceEntity()
            {
                Id = Guid.CreateVersion7(),
                ClientId = Guid.Parse("fc1c1662-cd80-4fab-b924-a39168765558"),
                Audience = "api://emessage"
            }
        ];
    }
}