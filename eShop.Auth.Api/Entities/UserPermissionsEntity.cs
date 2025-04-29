namespace eShop.Auth.Api.Entities;

public class UserPermissionsEntity : IEntity
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public UserEntity UserEntity { get; init; } = null!;
    public PermissionEntity PermissionEntity { get; init; } = null!;
    public DateTime CreateDate { get; set; }
    public DateTime UpdateDate { get; set; }
}