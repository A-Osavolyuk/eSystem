namespace eShop.Domain.Responses.API.Auth;

public class TrustDeviceResponse
{
    public Guid UserId { get; set; }
    public bool IsTwoFactorEnabled { get; set; }
    
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
}