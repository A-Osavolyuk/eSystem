namespace eSecurity.Core.Common.DTOs;

public class UserStateDto
{
    public Guid UserId { get; set; }
    
    public string? Username { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }

    public List<RoleDto> Roles { get; set; } = [];
    public List<PermissionDto> Permissions { get; set; } = [];
    
    public bool LockedOut { get; set; }
}