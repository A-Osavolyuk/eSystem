using eSecurity.Security.Authentication.TwoFactor;
using eSecurity.Security.Authorization.Access;
using eSecurity.Security.Authorization.Access.Verification;
using eSecurity.Security.Authorization.OAuth;
using eSecurity.Security.Authorization.OAuth.LinkedAccount;
using eSecurity.Security.Identity;
using eSystem.Core.Data.Entities;

namespace eSecurity.Data.Entities;

public class UserEntity : Entity
{
    public Guid Id { get; init; }
    public Guid? PersonalDataId { get; set; }

    public string Username { get; set; } = string.Empty;
    public string NormalizedUsername { get; set; } = string.Empty;
    public DateTimeOffset? UsernameChangeDate { get; set; }
    public bool AccountConfirmed { get; set; }
    public bool TwoFactorEnabled { get; set; }
    public int FailedLoginAttempts { get; set; }

    public int CodeResendAttempts { get; set; }
    public DateTimeOffset? CodeResendAvailableDate { get; set; }
    
    public PasswordEntity? Password { get; set; }
    public PersonalDataEntity? PersonalData { get; set; }
    public UserSecretEntity? Secret { get; set; }
    public UserLockoutStateEntity LockoutState { get; set; } = null!;
    public ICollection<ConsentEntity> Consents { get; set; } = null!;
    public ICollection<UserClientEntity> Clients { get; set; } = null!;
    public ICollection<UserEmailEntity> Emails { get; set; } = null!;
    public ICollection<UserPhoneNumberEntity> PhoneNumbers { get; set; } = null!;
    public ICollection<UserPermissionsEntity> Permissions { get; init; } = null!;
    public ICollection<UserRoleEntity> Roles { get; init; } = null!;
    public ICollection<UserRecoveryCodeEntity> RecoveryCodes { get; init; } = null!;
    public ICollection<UserTwoFactorMethodEntity> TwoFactorMethods { get; init; } = null!;
    public ICollection<UserLinkedAccountEntity> LinkedAccounts { get; init; } = null!;
    public ICollection<UserDeviceEntity> Devices { get; init; } = null!;
    public ICollection<UserVerificationMethodEntity> VerificationMethods { get; init; } = null!;

    public bool HasPassword() => Password is not null;
    public bool HasEmail(EmailType type) => Emails.Any(x => x.Type == type);
    public bool HasPhoneNumber(PhoneNumberType type) => PhoneNumbers.Any(x => x.Type == type);
    public bool HasLinkedAccounts() => LinkedAccounts.Count > 0;
    public bool HasLinkedAccount(LinkedAccountType type) => LinkedAccounts.Any(x => x.Type == type);
    public bool HasMethods() => TwoFactorMethods.Count > 0;
    public bool HasRecoveryCodes() => RecoveryCodes.Count > 0;
    public bool HasPasskeys() => Devices.Select(x => x.Passkey).Any(x => x is not null);
    public bool HasTwoFactor(TwoFactorMethod type) => TwoFactorMethods.Any(x => x.Method == type);
    public bool HasVerification(VerificationMethod method)
        => VerificationMethods.Any(x => x.Method == method);
}