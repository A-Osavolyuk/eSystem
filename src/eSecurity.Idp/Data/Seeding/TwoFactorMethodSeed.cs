using eSecurity.Core.Security.Authentication.TwoFactor;
using eSecurity.Idp.Data.Entities;
using eSystem.Core.Server.Data.Seeding;

namespace eSecurity.Idp.Data.Seeding;

public sealed class TwoFactorMethodSeed : Seed<TwoFactorMethodEntity>
{
    public override List<TwoFactorMethodEntity> Get()
    {
        return
        [
            new TwoFactorMethodEntity()
            {
                Id = Guid.CreateVersion7(),
                Type = TwoFactorMethod.HardwareKey,
                Priority = 4,
                Name = "Hardware Key"
            },
            new TwoFactorMethodEntity()
            {
                Id = Guid.CreateVersion7(),
                Type = TwoFactorMethod.SoftwareKey,
                Priority = 3,
                Name = "Software Key"
            },
            new TwoFactorMethodEntity()
            {
                Id = Guid.CreateVersion7(),
                Type = TwoFactorMethod.AuthenticatorApp,
                Priority = 2,
                Name = "Authenticator App"
            },
            new TwoFactorMethodEntity()
            {
                Id = Guid.CreateVersion7(),
                Type = TwoFactorMethod.EmailOtp,
                Priority = 1,
                Name = "Email OTP"
            },
            new TwoFactorMethodEntity()
            {
                Id = Guid.CreateVersion7(),
                Type = TwoFactorMethod.SmsOtp,
                Priority = 0,
                Name = "SMS OTP"
            },
            new TwoFactorMethodEntity()
            {
                Id = Guid.CreateVersion7(),
                Type = TwoFactorMethod.RecoveryCode,
                Priority = 0,
                Name = "Recovery Code"
            }
        ];
    }
}