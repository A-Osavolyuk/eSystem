namespace eSecurity.Core.Common.Requests;

public sealed class ApproveDeviceCodeRequest
{
    public required string UserCode { get; set; }
    public Guid? SessionId { get; set; }
}