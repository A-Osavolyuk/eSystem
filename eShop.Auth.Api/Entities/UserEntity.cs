namespace eShop.Auth.Api.Entities;

public class UserEntity : Entity
{
    public Guid Id { get; init; }
    public Guid? PersonalDataId { get; set; }
    
    public string Username { get; set; } = string.Empty;
    public string NormalizedUsername { get; set; } = string.Empty;
    public DateTimeOffset? UsernameChangeDate { get; set; }
    
    public string PasswordHash { get; set; } = string.Empty;
    public DateTimeOffset? PasswordChangeDate { get; set; }
    
    public bool AccountConfirmed { get; set; }
    public int FailedLoginAttempts { get; set; }
    
    public int CodeResendAttempts { get; set; }
    public DateTimeOffset? CodeResendAvailableDate { get; set; }

    public ICollection<UserEmailEntity> Emails { get; set; } = null!;
    public ICollection<UserPhoneNumberEntity> PhoneNumbers { get; set; } = null!;
    public ICollection<UserPermissionsEntity> Permissions { get;  init; } = null!;
    public ICollection<UserRoleEntity> Roles { get; init; } = null!;
    public ICollection<RecoveryCodeEntity> RecoveryCodes { get; init; } = null!;
    public ICollection<UserTwoFactorProviderEntity> TwoFactorProviders { get; init; } = null!;
    public ICollection<UserLinkedAccountEntity> LinkedAccounts { get; init; } = null!;
    public ICollection<UserChangesEntity> Changes { get; init; } = null!;
    public ICollection<UserDeviceEntity> Devices { get; init; } = null!;
    public ICollection<UserPasskeyEntity> Passkeys { get; init; } = null!;
    public LockoutStateEntity LockoutState { get; set; } = null!;
    public PersonalDataEntity? PersonalData { get; set; } = null!;

    public UserEmailEntity? PrimaryEmail 
        => Emails.FirstOrDefault(x => x.Type == EmailType.Primary);
    public UserEmailEntity? RecoveryEmail 
        => Emails.FirstOrDefault(x => x.Type == EmailType.Recovery);
    public UserPhoneNumberEntity? PrimaryPhoneNumber 
        => PhoneNumbers.FirstOrDefault(x => x.Type == PhoneNumberType.Primary);
    public UserPhoneNumberEntity? RecoveryPhoneNumber 
        => PhoneNumbers.FirstOrDefault(x => x.Type == PhoneNumberType.Recovery);

    public bool HasPassword() => !string.IsNullOrEmpty(PasswordHash);
    public bool HasPrimaryEmail() => Emails.Any(x => x.Type is EmailType.Primary);
    public bool HasRecoveryEmail() => Emails.Any(x => x.Type is EmailType.Recovery);
    public bool HasPrimaryPhoneNumber() => PhoneNumbers.Any(x => x.Type is PhoneNumberType.Primary);
    public bool HasRecoveryPhoneNumber() => PhoneNumbers.Any(x => x.Type is PhoneNumberType.Recovery);
    public bool HasLinkedAccount() => LinkedAccounts.Any(x => x.Allowed);
    public bool HasTwoFactor() => TwoFactorProviders.Any(x => x.Subscribed);
    public bool HasRecoveryCodes() => RecoveryCodes.Count > 0;
}