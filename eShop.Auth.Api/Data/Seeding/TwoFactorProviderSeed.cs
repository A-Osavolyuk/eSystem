using eShop.Domain.Abstraction.Data.Seeding;
using eShop.Domain.Common.Security.Constants;

namespace eShop.Auth.Api.Data.Seeding;

public class TwoFactorProviderSeed : Seed<TwoFactorProviderEntity>
{
    public override List<TwoFactorProviderEntity> Get()
    {
        return
        [
            new TwoFactorProviderEntity()
            {
                Id = Guid.Parse("a4d155e3-1746-4f4d-af0e-e5ebca3ffcac"),
                Name = ProviderTypes.Email,
                CreateDate = DateTime.UtcNow,
                UpdateDate = null
            },
            new TwoFactorProviderEntity()
            {
                Id = Guid.Parse("60c95fd9-c0e1-4a08-8d21-50c66f3a91d4"),
                Name = ProviderTypes.Sms,
                CreateDate = DateTime.UtcNow,
                UpdateDate = null
            },
            new TwoFactorProviderEntity()
            {
                Id = Guid.Parse("f01b5a5a-8f4b-4814-8ade-2935e02af0a5"),
                Name = ProviderTypes.Authenticator,
                CreateDate = DateTime.UtcNow,
                UpdateDate = null
            }
        ];
    }
}