namespace eSystem.Core.Responses.Auth;

public class TrustDeviceResponse
{
    public Guid UserId { get; set; }
    public bool IsTwoFactorEnabled { get; set; }
}