using Microsoft.AspNetCore.Mvc;

namespace eSystem.Core.Security.Authorization.OAuth.Token;

public abstract class TokenRequest
{
    [FromForm(Name = "grant_type")]
    public required string GrantType { get; set; }
    
    [FromForm(Name = "client_id")]
    public required string ClientId { get; set; }
    
    [FromForm(Name = "client_secret")]
    public string? ClientSecret { get; set; }
    
    [FromForm(Name = "client_assertion")]
    public string? ClientAssertion { get; set; }
    
    [FromForm(Name = "client_assertion_type")]
    public string? ClientAssertionType { get; set; }
}