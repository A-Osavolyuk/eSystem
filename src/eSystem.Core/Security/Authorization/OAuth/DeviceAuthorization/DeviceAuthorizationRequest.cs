using Microsoft.AspNetCore.Mvc;

namespace eSystem.Core.Security.Authorization.OAuth.DeviceAuthorization;

public class DeviceAuthorizationRequest
{
    [FromForm(Name = "client_id")]
    public required string ClientId { get; set; }
    
    [FromForm(Name = "scope")]
    public required string Scope { get; set; }
    
    [FromForm(Name = "acr_values")]
    public string[]? AcrValues { get; set; }
    
    [FromForm(Name = "device_name")] 
    public string? DeviceName { get; set; }
    
    [FromForm(Name = "device_model")] 
    public string? DeviceModel { get; set; }
}