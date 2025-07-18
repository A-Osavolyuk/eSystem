namespace eShop.Domain.Responses.API.Auth;

public record LoginResponse
{
    public Guid UserId { get; set; }
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public bool TwoFactorEnabled { get; set; } = false;
    public bool IsLockedOut { get; set; }
    public int FailedLoginAttempts { get; set; }
    public LockoutReasonDto? Reason { get; set; }
}
