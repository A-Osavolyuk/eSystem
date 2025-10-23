namespace eSystem.Domain.DTOs;

public class UserStateDto
{
    public Guid UserId { get; set; }
    
    public string? Username { get; set; }
    
    public string? PrimaryEmail { get; set; }
    public string? RecoveryEmail { get; set; }
    
    public string? PrimaryPhoneNumber { get; set; }
    public string? RecoveryPhoneNumber { get; set; }

    public List<RoleDto> Roles { get; set; } = [];
    public List<PermissionDto> Permissions { get; set; } = [];
    
    public bool LockedOut { get; set; }
}