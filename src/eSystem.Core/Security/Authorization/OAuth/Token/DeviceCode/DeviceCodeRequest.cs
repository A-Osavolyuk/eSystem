using Microsoft.AspNetCore.Mvc;

namespace eSystem.Core.Security.Authorization.OAuth.Token.DeviceCode;

public sealed class DeviceCodeRequest : TokenRequest
{
    [FromForm(Name = "device_code")]
    public string? DeviceCode { get; set; }
}