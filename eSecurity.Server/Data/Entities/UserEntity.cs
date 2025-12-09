using eSecurity.Core.Security.Authorization.OAuth;
using eSystem.Core.Data.Entities;

namespace eSecurity.Server.Data.Entities;

public class UserEntity : Entity
{
    public Guid Id { get; init; }
    public string Username { get; set; } = string.Empty;
    public string NormalizedUsername { get; set; } = string.Empty;
    public DateTimeOffset? UsernameChangeDate { get; set; }
    public bool AccountConfirmed { get; set; }
    public int FailedLoginAttempts { get; set; }

    public int CodeResendAttempts { get; set; }
    public DateTimeOffset? CodeResendAvailableDate { get; set; }

    public string ZoneInfo { get; set; } = string.Empty;
    public string Locale { get; set; } = string.Empty;
    
    public PersonalDataEntity? PersonalData { get; set; }
    public UserSecretEntity? Secret { get; set; }
    public UserLockoutStateEntity LockoutState { get; set; } = null!;
    public ICollection<UserPermissionsEntity> Permissions { get; init; } = null!;
    public ICollection<UserRoleEntity> Roles { get; init; } = null!;
    public ICollection<UserDeviceEntity> Devices { get; init; } = null!;
}