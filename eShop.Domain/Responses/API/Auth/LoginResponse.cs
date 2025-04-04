namespace eShop.Domain.Responses.Api.Auth;

public record LoginResponse
{
    public User User { get; set; } = null!;
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool HasTwoFactorAuthentication { get; set; } = false;
}
