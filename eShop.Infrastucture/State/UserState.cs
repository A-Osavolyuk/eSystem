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
    
    public override async Task Change()
    {
        await StateChanged();
    }
}