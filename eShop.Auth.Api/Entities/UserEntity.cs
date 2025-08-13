namespace eShop.Auth.Api.Entities;

public class UserEntity : Entity
{
    public Guid Id { get; init; }
    
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
    
    public string UserName { get; set; } = string.Empty;
    public string NormalizedUserName { get; set; } = string.Empty;
    public DateTimeOffset? UserNameChangeDate { get; set; }
    
    public string? PhoneNumber { get; set; } = string.Empty;
    public bool PhoneNumberConfirmed { get; set; }
    public DateTimeOffset? PhoneNumberConfirmationDate { get; set; }
    public DateTimeOffset? PhoneNumberChangeDate { get; set; }
    
    public string PasswordHash { get; set; } = string.Empty;
    public DateTimeOffset? PasswordChangeDate { get; set; }
    
    public bool AccountConfirmed { get; set; }
    public int FailedLoginAttempts { get; set; }
    
    public ICollection<UserPermissionsEntity> Permissions { get;  init; } = null!;
    public ICollection<UserRoleEntity> Roles { get; init; } = null!;
    public ICollection<RecoveryCodeEntity> RecoveryCodes { get; init; } = null!;
    public ICollection<UserProviderEntity> Providers { get; init; } = null!;
    public ICollection<UserOAuthProviderEntity> OAuthProviders { get; init; } = null!;
    public ICollection<UserChangesEntity> Changes { get; init; } = null!;
    public ICollection<UserDeviceEntity> Devices { get; init; } = null!;
    public LockoutStateEntity LockoutState { get; set; } = null!;
    public PersonalDataEntity PersonalData { get; set; } = null!;
}