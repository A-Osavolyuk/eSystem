namespace eShop.Domain.Responses.API.Auth;

public record LoginResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool HasTwoFactorAuthentication { get; set; } = false;
}
