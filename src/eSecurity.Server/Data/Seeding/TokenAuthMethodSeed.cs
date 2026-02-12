using eSecurity.Server.Data.Entities;
using eSecurity.Server.Security.Authorization.Constants;
using eSystem.Core.Data.Seeding;

namespace eSecurity.Server.Data.Seeding;

public sealed class TokenAuthMethodSeed : Seed<TokenAuthMethodEntity>
{
    public override List<TokenAuthMethodEntity> Get()
    {
        return
        [
            new TokenAuthMethodEntity()
            {
                Id = Guid.Parse("ebef61b5-e5cb-48d5-82f1-eeb6e2104992"),
                Method = TokenAuthMethods.None
            },
            new TokenAuthMethodEntity()
            {
                Id = Guid.Parse("91540f91-7588-4167-848b-16e4d6784d26"),
                Method = TokenAuthMethods.ClientSecretBasic
            },
            new TokenAuthMethodEntity()
            {
                Id = Guid.Parse("ec1fc83b-139d-4047-bd38-50191a621fbc"),
                Method = TokenAuthMethods.ClientSecretPost
            },
            new TokenAuthMethodEntity()
            {
                Id = Guid.Parse("ef1cc163-709a-4048-b20e-fb4be66bc735"),
                Method = TokenAuthMethods.ClientSecretJwt
            },
            new TokenAuthMethodEntity()
            {
                Id = Guid.Parse("90abaf6d-b87d-41cb-a214-69b3dead6370"),
                Method = TokenAuthMethods.PrivateKeyJwt
            },
        ];
    }
}