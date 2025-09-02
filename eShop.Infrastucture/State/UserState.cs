using eShop.Domain.Abstraction.State;
using eShop.Domain.DTOs;

namespace eShop.Infrastructure.State;

public class UserState : AsyncState
{
    public Guid UserId { get; set; }
    
    public string? Username { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? RecoveryEmail { get; set; }

    public List<RoleDto> Roles { get; set; } = [];
    public List<PermissionDto> Permissions { get; set; } = [];
    
    public bool IsAuthenticated => UserId != Guid.Empty;

    public bool HasRole(string role) 
        => Roles.Any(x => x.Name == role);
    public bool HasRole(List<string> roles) 
        => roles.Intersect(Roles.Select(x => x.Name)).Any();
    public bool HasPermission(string permission) 
        => Permissions.Any(x => x.Name == permission);
    public bool HasPermission(List<string> permissions) 
        => permissions.Intersect(Permissions.Select(x => x.Name)).Any();
    
    public override async Task Change()
    {
        await StateChanged();
    }
}