using eShop.Domain.Abstraction.Data.Seeding;
using eShop.Domain.Common.Security.Constants;

namespace eShop.Auth.Api.Data.Seeding;

public class VerificationProviderSeed : Seed<VerificationProviderEntity>
{
    public override List<VerificationProviderEntity> Get()
    {
        return
        [
            new ()
            {
                Id = Guid.Parse("1eddc929-568d-43ce-a622-51eb733bd8d6"),
                Name = ProviderTypes.Email,
                CreateDate = DateTimeOffset.UtcNow
            },
            new ()
            {
                Id = Guid.Parse("5c2c2f81-00f0-4f93-b40f-8bc7ffbf19e0"),
                Name = ProviderTypes.Sms,
                CreateDate = DateTimeOffset.UtcNow
            },
            new ()
            {
                Id = Guid.Parse("42527e73-2c6c-4817-8d8d-310e735b73be"),
                Name = ProviderTypes.Authenticator,
                CreateDate = DateTimeOffset.UtcNow
            }
        ];
    }
}