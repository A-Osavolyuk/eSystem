namespace eShop.Auth.Api.Entities;

public class UserPermissionsEntity : IAuditable, IIdentifiable<Guid>
{
    public Guid Id { get; init; }
    public string UserId { get; init; } = string.Empty;
    public AppUser User { get; init; } = null!;
    public PermissionEntity PermissionEntity { get; init; } = null!;
    public DateTime CreateDate { get; init; }
    public DateTime UpdateDate { get; init; }
}