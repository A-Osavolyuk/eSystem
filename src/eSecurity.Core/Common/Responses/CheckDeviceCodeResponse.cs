namespace eSecurity.Core.Common.Responses;

public sealed class CheckDeviceCodeResponse
{
    public bool Exists { get; set; }
    public bool IsDenied { get; set; }
    public bool IsActivated { get; set; }
    public bool IsConsumed { get; set; }
    public bool IsExpired { get; set; }
}