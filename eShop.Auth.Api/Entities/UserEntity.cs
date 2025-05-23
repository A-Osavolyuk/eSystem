namespace eShop.Auth.Api.Entities;

public class UserEntity : IEntity<Guid>
{
    public Guid Id { get; init; }
    public Guid? PersonalDataId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string NormalizedEmail { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string NormalizedUserName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public bool PhoneNumberConfirmed { get; set; }
    public bool EmailConfirmed { get; set; }
    public bool AccountConfirmed { get; set; }
    public bool TwoFactorEnabled { get; set; }
    public string PasswordHash { get; set; } = string.Empty;
    public DateTimeOffset? CreateDate { get; set; }
    public DateTimeOffset? UpdateDate { get; set; }
    
    public PersonalDataEntity? PersonalData { get; init; }
    public ICollection<UserPermissionsEntity> Permissions { get;  init; } = null!;
    public ICollection<UserRoleEntity> Roles { get; init; } = null!;
}