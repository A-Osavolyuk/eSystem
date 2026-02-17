namespace eSecurity.Core.Common.Requests;

public sealed class DenyDeviceCodeRequest
{
    public required string UserCode { get; set; }
    public required string Subject { get; set; }
    public Guid? SessionId { get; set; }
}