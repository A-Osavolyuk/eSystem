namespace eSecurity.Common.Responses;

public class TrustDeviceResponse
{
    public Guid UserId { get; set; }
    public bool IsTwoFactorEnabled { get; set; }
}