using eSecurity.Server.Data.Entities;
using eSystem.Core.Data.Seeding;
using eSystem.Core.Security.Authentication.OpenIdConnect.Constants;

namespace eSecurity.Server.Data.Seeding;

public class ClientResponseTypeSeed : Seed<ClientResponseTypeEntity>
{
    public override List<ClientResponseTypeEntity> Get()
    {
        return
        [
            // eSecurity
            new ClientResponseTypeEntity()
            {
                ClientId = Guid.Parse("392e390f-33bd-4f30-af70-ccbe04bbb2c4"),
                ResponseTypeId = Guid.Parse("690d364a-056b-4bbf-ab24-5d3835ff3917")
            },

            // eCinema
            new ClientResponseTypeEntity()
            {
                ClientId = Guid.Parse("307268b0-005c-4ee4-a0e8-a93bd0010382"),
                ResponseTypeId = Guid.Parse("690d364a-056b-4bbf-ab24-5d3835ff3917")
            },

            // eSecurity
            new ClientResponseTypeEntity()
            {
                ClientId = Guid.Parse("fc1c1662-cd80-4fab-b924-a39168765558"),
                ResponseTypeId = Guid.Parse("690d364a-056b-4bbf-ab24-5d3835ff3917")
            }
        ];
    }
}