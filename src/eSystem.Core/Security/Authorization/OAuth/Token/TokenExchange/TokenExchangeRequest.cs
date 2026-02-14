using Microsoft.AspNetCore.Mvc;

namespace eSystem.Core.Security.Authorization.OAuth.Token.TokenExchange;

public sealed class TokenExchangeRequest : TokenRequest
{
    [FromForm(Name = "actor_token")]
    public string? ActorToken { get; set; }
    
    [FromForm(Name = "actor_token_type")]
    public string? ActorTokenType { get; set; }
    
    [FromForm(Name = "subject_token")]
    public string? SubjectToken { get; set; }
    
    [FromForm(Name = "subject_token_type")]
    public string? SubjectTokenType { get; set; }
    
    [FromForm(Name = "request_token_type")]
    public string? RequestTokenType { get; set; }
    
    [FromForm(Name = "scope")]
    public string? Scope { get; set; }
    
    [FromForm(Name = "audience")]
    public string? Audience { get; set; }
}