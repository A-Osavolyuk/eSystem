using eSecurity.Idp.Data.Entities;
using eSystem.Core.Security.Authentication.OpenIdConnect;
using eSystem.Core.Server.Data.Seeding;

namespace eSecurity.Idp.Data.Seeding;

public sealed class ResponseTypeSeed : Seed<ResponseTypeEntity>
{
    public override List<ResponseTypeEntity> Get()
    {
        return
        [
            new ResponseTypeEntity()
            {
                Id = Guid.Parse("690d364a-056b-4bbf-ab24-5d3835ff3917"),
                Type = ResponseType.Code
            }
        ];
    }
}