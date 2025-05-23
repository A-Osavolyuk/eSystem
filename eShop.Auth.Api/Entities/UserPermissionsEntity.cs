namespace eShop.Auth.Api.Entities;

public class UserPermissionsEntity : IEntity
{
    public Guid PermissionId { get; init; }
    public Guid UserId { get; init; }

    public DateTimeOffset? CreateDate { get; set; }
    public DateTimeOffset? UpdateDate { get; set; }

    public UserEntity User { get; set; } = null!;
    public PermissionEntity Permission { get; set; } = null!;
}