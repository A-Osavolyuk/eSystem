using eSystem.Core.Security.Authorization.OAuth;
using Microsoft.AspNetCore.Mvc;

namespace eSystem.Core.Server.Security.Authorization.OAuth.Introspection;

public sealed class IntrospectionRequest
{
    [FromForm(Name = "token")]
    public string Token { get; init; } = null!;
    
    [FromForm(Name = "token_type_hint")]
    public TokenTypeHint? TokenTypeHint { get; init; }
}