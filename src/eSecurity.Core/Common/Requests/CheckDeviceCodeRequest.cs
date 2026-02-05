namespace eSecurity.Core.Common.Requests;

public sealed class CheckDeviceCodeRequest
{
    public required string UserCode { get; set; }
}