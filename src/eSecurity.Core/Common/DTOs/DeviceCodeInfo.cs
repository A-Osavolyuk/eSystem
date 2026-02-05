namespace eSecurity.Core.Common.DTOs;

public sealed class DeviceCodeInfo
{
    public required string ClientName { get; set; }
    public string? DeviceModel { get; set; }
    public required string[] Scopes { get; set; }
}