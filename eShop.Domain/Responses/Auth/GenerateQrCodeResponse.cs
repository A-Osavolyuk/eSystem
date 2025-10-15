namespace eShop.Domain.Responses.Auth;

public class GenerateQrCodeResponse
{
    public required string QrCode { get; set; }
    public required string Secret { get; set; }
}