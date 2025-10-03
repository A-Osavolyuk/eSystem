using eShop.Domain.Abstraction.Data.Seeding;

namespace eShop.Auth.Api.Data.Seeding;

public class TwoFactorMethodeed : Seed<TwoFactorMethodEntity>
{
    public override List<TwoFactorMethodEntity> Get()
    {
        return
        [
            new TwoFactorMethodEntity()
            {
                Id = Guid.Parse("60c95fd9-c0e1-4a08-8d21-50c66f3a91d4"),
                Type = MethodType.AuthenticatorApp,
                CreateDate = DateTime.UtcNow,
            },
            new TwoFactorMethodEntity()
            {
                Id = Guid.Parse("f01b5a5a-8f4b-4814-8ade-2935e02af0a5"),
                Type = MethodType.Passkey,
                CreateDate = DateTime.UtcNow,
            },
            new TwoFactorMethodEntity()
            {
                Id = Guid.Parse("91fa9bbf-e0bb-4dce-80e5-fb1981feb92d"),
                Type = MethodType.Sms,
                CreateDate = DateTime.UtcNow,
            }
        ];
    }
}