namespace eSecurity.Core.Common.Requests;

public class RegenerateQrCodeRequest
{
    public required Guid UserId { get; set; }
}