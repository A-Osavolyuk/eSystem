using eShop.Domain.Abstraction.Data.Seeding;

namespace eShop.Auth.Api.Data.Seeding;

public class TwoFactorProviderSeed : Seed<TwoFactorProviderEntity>
{
    public override List<TwoFactorProviderEntity> Get()
    {
        return
        [
            new TwoFactorProviderEntity()
            {
                Id = Guid.Parse("60c95fd9-c0e1-4a08-8d21-50c66f3a91d4"),
                Type = ProviderType.AuthenticatorApp,
                CreateDate = DateTime.UtcNow,
            },
            new TwoFactorProviderEntity()
            {
                Id = Guid.Parse("f01b5a5a-8f4b-4814-8ade-2935e02af0a5"),
                Type = ProviderType.Passkey,
                CreateDate = DateTime.UtcNow,
            },
            new TwoFactorProviderEntity()
            {
                Id = Guid.Parse("91fa9bbf-e0bb-4dce-80e5-fb1981feb92d"),
                Type = ProviderType.Sms,
                CreateDate = DateTime.UtcNow,
            }
        ];
    }
}