namespace eShop.Domain.Responses.API.Auth;

public record LoginResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool TwoFactorEnabled { get; set; } = false;
    public bool IsLockedOut { get; set; }
    public string? Reason { get; set; }
    public string? Code { get; set; }
    public int FailedLoginAttempts { get; set; }
    public Guid UserId { get; set; }
}
