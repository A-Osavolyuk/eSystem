namespace eShop.Domain.Responses.API.Auth;

public class TrustDeviceResponse
{
    public Guid UserId { get; set; }
    public bool IsTwoFactorEnabled { get; set; }
    
    public string? Token { get; set; }
}