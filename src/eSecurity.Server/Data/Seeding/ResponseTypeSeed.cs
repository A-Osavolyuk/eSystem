using eSecurity.Server.Data.Entities;
using eSystem.Core.Data.Seeding;
using eSystem.Core.Security.Authentication.OpenIdConnect.Constants;

namespace eSecurity.Server.Data.Seeding;

public sealed class ResponseTypeSeed : Seed<ResponseTypeEntity>
{
    public override List<ResponseTypeEntity> Get()
    {
        return
        [
            new ResponseTypeEntity()
            {
                Id = Guid.Parse("690d364a-056b-4bbf-ab24-5d3835ff3917"),
                Type = ResponseTypes.Code
            }
        ];
    }
}