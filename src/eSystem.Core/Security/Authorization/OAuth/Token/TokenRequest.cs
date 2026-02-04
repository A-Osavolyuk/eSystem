using Microsoft.AspNetCore.Mvc;

namespace eSystem.Core.Security.Authorization.OAuth.Token;

public class TokenRequest
{
    [FromForm(Name = "grant_type")]
    public required string GrantType { get; set; }
    
    [FromForm(Name = "client_id")]
    public required string ClientId { get; set; }
    
    [FromForm(Name = "redirect_uri")]
    public string? RedirectUri { get; set; }
    
    [FromForm(Name = "refresh_token")]
    public string? RefreshToken { get; set; }
    
    [FromForm(Name = "code")]
    public string? Code { get; set; }
    
    [FromForm(Name = "device_code")]
    public string? DeviceCode { get; set; }
    
    [FromForm(Name = "client_secret")]
    public string? ClientSecret { get; set; }
    
    [FromForm(Name = "code_verifier")]
    public string? CodeVerifier { get; set; }
    
    [FromForm(Name = "scope")]
    public string? Scope { get; set; }
    
    [FromForm(Name = "client_assertion")]
    public string? ClientAssertion { get; set; }
    
    [FromForm(Name = "client_assertion_type")]
    public string? ClientAssertionType { get; set; }
}