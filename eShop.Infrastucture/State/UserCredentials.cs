using eShop.Domain.DTOs;

namespace eShop.Infrastructure.State;

public class UserCredentials
{
    public string? Email { get; set; }
    public string? Username { get; set; }
    public string? PhoneNumber { get; set; }
    public string? RecoveryEmail { get; set; }
}