using eSystem.Core.Enums;
using Microsoft.AspNetCore.Mvc;

namespace eSystem.Core.Server.Security.Authorization.OAuth.Token.TokenExchange;

public sealed class TokenExchangeRequest : TokenRequest
{
    [FromForm(Name = "actor_token")]
    public string? ActorToken { get; set; }
    
    [FromForm(Name = "actor_token_type")]
    public TokenType? ActorTokenType { get; set; }
    
    [FromForm(Name = "subject_token")]
    public string? SubjectToken { get; set; }
    
    [FromForm(Name = "subject_token_type")]
    public TokenType? SubjectTokenType { get; set; }
    
    [FromForm(Name = "request_token_type")]
    public TokenType? RequestTokenType { get; set; }
    
    [FromForm(Name = "scope")]
    public string? Scope { get; set; }
    
    [FromForm(Name = "audience")]
    public string? Audience { get; set; }

    public override Dictionary<string, string> GetForm()
    {
        var form = base.GetForm();
        if (!string.IsNullOrEmpty(ActorToken))
            form["actor_token"] = ActorToken;
        
        if (ActorTokenType.HasValue)
            form["actor_token_type"] = ActorTokenType.Value.GetString();
        
        if (!string.IsNullOrEmpty(SubjectToken))
            form["subject_token"] = SubjectToken;
        
        if (SubjectTokenType.HasValue)
            form["subject_token_type"] = SubjectTokenType.Value.GetString();
        
        if (RequestTokenType.HasValue)
            form["request_token_type"] = RequestTokenType.Value.GetString();
        
        if (!string.IsNullOrEmpty(Scope))
            form["scope"] = Scope;
        
        if (!string.IsNullOrEmpty(Audience))
            form["audience"] = Audience;
        
        return form;
    }
}