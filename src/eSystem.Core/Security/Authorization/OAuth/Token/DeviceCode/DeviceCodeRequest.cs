using Microsoft.AspNetCore.Mvc;

namespace eSystem.Core.Security.Authorization.OAuth.Token.DeviceCode;

public sealed class DeviceCodeRequest : TokenRequest
{
    [FromForm(Name = "device_code")]
    public string? DeviceCode { get; init; }

    public override Dictionary<string, string> GetForm()
    {
        var form = base.GetForm();
        if (!string.IsNullOrEmpty(DeviceCode))
            form["device_code"] = DeviceCode;

        return form;
    }
}