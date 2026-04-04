using eSecurity.Server.Data.Entities;
using eSystem.Core.Data.Seeding;
using eSystem.Core.Security.Authentication.OpenIdConnect;

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
                Method = TokenAuthMethod.None
            },
            new TokenAuthMethodEntity()
            {
                Id = Guid.Parse("91540f91-7588-4167-848b-16e4d6784d26"),
                Method = TokenAuthMethod.ClientSecretBasic
            },
            new TokenAuthMethodEntity()
            {
                Id = Guid.Parse("ec1fc83b-139d-4047-bd38-50191a621fbc"),
                Method = TokenAuthMethod.ClientSecretPost
            },
            new TokenAuthMethodEntity()
            {
                Id = Guid.Parse("ef1cc163-709a-4048-b20e-fb4be66bc735"),
                Method = TokenAuthMethod.ClientSecretJwt
            },
            new TokenAuthMethodEntity()
            {
                Id = Guid.Parse("90abaf6d-b87d-41cb-a214-69b3dead6370"),
                Method = TokenAuthMethod.PrivateKeyJwt
            },
        ];
    }
}