using Microsoft.AspNetCore.Mvc;

namespace eSystem.Core.Security.Authorization.OAuth.Introspection;

public class IntrospectionRequest
{
    [FromForm(Name = "token")]
    public required string Token { get; set; }
    
    [FromForm(Name = "token_type_hint")]
    public TokenTypeHint? TokenTypeHint { get; set; }
}