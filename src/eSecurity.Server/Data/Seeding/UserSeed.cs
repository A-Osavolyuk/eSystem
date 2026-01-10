using eSecurity.Server.Data.Entities;
using eSystem.Core.Common.Localization.Locale;
using eSystem.Core.Common.Localization.Time;
using eSystem.Core.Data.Seeding;

namespace eSecurity.Server.Data.Seeding;

public class UserSeed : Seed<UserEntity>
{
    public override List<UserEntity> Get()
    {
        return
        [
            new ()
            {
                Id = Guid.Parse("188c7286-b0b9-4cb1-8f7f-503c6349bc65"),
                Username = "pipidastr",
                NormalizedUsername = "PIPIDASTR".ToUpper(),
                AccountConfirmed = true,
                ZoneInfo = IanaTimeZones.Europe.Kiev,
                Locale = Locales.Other.UkrainianUkraine
            }
        ];
    }
}