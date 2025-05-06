namespace eShop.Auth.Api.Entities;

public class UserPermissionsEntity : IEntity<Guid>
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }

    public DateTime? CreateDate { get; set; }
    public DateTime? UpdateDate { get; set; }

    public UserEntity User { get; set; } = null!;
    public PermissionEntity Permission { get; set; } = null!;
}