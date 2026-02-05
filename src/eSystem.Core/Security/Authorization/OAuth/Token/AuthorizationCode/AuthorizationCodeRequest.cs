using Microsoft.AspNetCore.Mvc;

namespace eSystem.Core.Security.Authorization.OAuth.Token.AuthorizationCode;

public sealed class AuthorizationCodeRequest : TokenRequest
{
    [FromForm(Name = "code")]
    public string? Code { get; set; }
    
    [FromForm(Name = "code_verifier")]
    public string? CodeVerifier { get; set; }
    
    [FromForm(Name = "redirect_uri")]
    public string? RedirectUri { get; set; }
}