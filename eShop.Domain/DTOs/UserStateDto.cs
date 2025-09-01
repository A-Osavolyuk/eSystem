namespace eShop.Domain.DTOs;

public class UserStateDto
{
    public Guid UserId { get; set; }
    
    public string? Username { get; set; }
    public string? Email { get; set; }
    public string? RecoveryEmail { get; set; }
    public string? PhoneNumber { get; set; }
    
    public bool LockedOut { get; set; }
}