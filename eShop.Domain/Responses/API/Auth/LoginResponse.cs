namespace eShop.Domain.Responses.API.Auth;

public record LoginResponse
{
    public Guid UserId { get; set; }
    public Guid? DeviceId { get; set; }
    public string? Email { get; set; }
    public bool IsEmailConfirmed { get; set; }
    public bool IsDeviceTrusted { get; set; }
    public bool IsDeviceBlocked { get; set; }
    public bool IsTwoFactorEnabled { get; set; }
    
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    
    public bool IsLockedOut { get; set; }
    public LockoutReasonDto? Reason { get; set; }
    
    public int MaxFailedLoginAttempts { get; set; }
    public int FailedLoginAttempts { get; set; }
}
