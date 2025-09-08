namespace eShop.Auth.Api.Entities;

public class UserEntity : Entity
{
    public Guid Id { get; init; }
    public Guid? PersonalDataId { get; set; }
    
    public string Email { get; set; } = string.Empty;
    public string NormalizedEmail { get; set; } = string.Empty;
    public bool EmailConfirmed { get; set; }
    public DateTimeOffset? EmailConfirmationDate { get; set; }
    public DateTimeOffset? EmailChangeDate { get; set; }
    
    public string? RecoveryEmail { get; set; } = string.Empty;
    public string? NormalizedRecoveryEmail { get; set; } = string.Empty;
    public bool RecoveryEmailConfirmed { get; set; }
    public DateTimeOffset? RecoveryEmailConfirmationDate { get; set; }
    public DateTimeOffset? RecoveryEmailChangeDate { get; set; }
    
    public string Username { get; set; } = string.Empty;
    public string NormalizedUsername { get; set; } = string.Empty;
    public DateTimeOffset? UsernameChangeDate { get; set; }
    
    public string? PhoneNumber { get; set; } = string.Empty;
    public bool PhoneNumberConfirmed { get; set; }
    public DateTimeOffset? PhoneNumberConfirmationDate { get; set; }
    public DateTimeOffset? PhoneNumberChangeDate { get; set; }
    
    public string PasswordHash { get; set; } = string.Empty;
    public DateTimeOffset? PasswordChangeDate { get; set; }
    
    public bool AccountConfirmed { get; set; }
    public int FailedLoginAttempts { get; set; }

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

    public bool HasPassword() => !string.IsNullOrEmpty(PasswordHash);
    public bool HasEmail() => Emails.Any(x => x.IsPrimary);
    public bool HasRecoveryEmail() => Emails.Any(x => x.IsRecovery);
    public bool HasPhoneNumber() => PhoneNumbers.Any(x => x.IsPrimary);
    public bool HasLinkedAccount() => LinkedAccounts.Any(x => x.Allowed);
    public bool HasTwoFactor() => TwoFactorProviders.Any(x => x.Subscribed);
}