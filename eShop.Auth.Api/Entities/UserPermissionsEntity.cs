using eShop.Domain.Interfaces;

namespace eShop.Auth.Api.Entities;

public class UserPermissionsEntity : IAuditable, IIdentifiable<Guid>
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public AppUser User { get; init; } = null!;
    public PermissionEntity PermissionEntity { get; init; } = null!;
    public DateTime CreateDate { get; set; }
    public DateTime UpdateDate { get; set; }
}