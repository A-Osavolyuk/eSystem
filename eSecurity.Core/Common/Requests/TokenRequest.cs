using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace eSecurity.Core.Common.Requests;

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
    
    [FromForm(Name = "client_secret")]
    public string? ClientSecret { get; set; }
    
    [FromForm(Name = "code_verifier")]
    public string? CodeVerifier { get; set; }
    
    [FromForm(Name = "scope")]
    public string? Scope { get; set; }
}