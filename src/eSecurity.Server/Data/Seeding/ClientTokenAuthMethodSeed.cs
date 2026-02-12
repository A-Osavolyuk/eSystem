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
                ClientId = Guid.Parse("392e390f-33bd-4f30-af70-ccbe04bbb2c4"),
                MethodId = Guid.Parse("91540f91-7588-4167-848b-16e4d6784d26")
            },
            new ClientTokenAuthMethodEntity()
            {
                ClientId = Guid.Parse("392e390f-33bd-4f30-af70-ccbe04bbb2c4"),
                MethodId = Guid.Parse("ec1fc83b-139d-4047-bd38-50191a621fbc")
            },
            
            // eCinema
            new ClientTokenAuthMethodEntity()
            {
                ClientId = Guid.Parse("307268b0-005c-4ee4-a0e8-a93bd0010382"),
                MethodId = Guid.Parse("91540f91-7588-4167-848b-16e4d6784d26")
            },
            new ClientTokenAuthMethodEntity()
            {
                ClientId = Guid.Parse("307268b0-005c-4ee4-a0e8-a93bd0010382"),
                MethodId = Guid.Parse("ec1fc83b-139d-4047-bd38-50191a621fbc")
            },
            
            // eCinema TV App
            new ClientTokenAuthMethodEntity()
            {
                ClientId = Guid.Parse("19362a03-8793-4300-82ad-28719f21a8e2"),
                MethodId = Guid.Parse("91540f91-7588-4167-848b-16e4d6784d26")
            },
            new ClientTokenAuthMethodEntity()
            {
                ClientId = Guid.Parse("19362a03-8793-4300-82ad-28719f21a8e2"),
                MethodId = Guid.Parse("ec1fc83b-139d-4047-bd38-50191a621fbc")
            },
            
            // eMessage
            new ClientTokenAuthMethodEntity()
            {
                ClientId = Guid.Parse("fc1c1662-cd80-4fab-b924-a39168765558"),
                MethodId = Guid.Parse("91540f91-7588-4167-848b-16e4d6784d26")
            },
            new ClientTokenAuthMethodEntity()
            {
                ClientId = Guid.Parse("fc1c1662-cd80-4fab-b924-a39168765558"),
                MethodId = Guid.Parse("ec1fc83b-139d-4047-bd38-50191a621fbc")
            },
        ];
    }
}