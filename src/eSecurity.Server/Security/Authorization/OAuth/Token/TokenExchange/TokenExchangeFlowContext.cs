using eSystem.Core.Security.Authorization.OAuth;

namespace eSecurity.Server.Security.Authorization.OAuth.Token.TokenExchange;

public sealed class TokenExchangeFlowContext
{
    public required string ClientId { get; set; }
    public required GrantType GrantType { get; set; }
    public required string SubjectToken { get; set; }
    public required TokenType SubjectTokenType { get; set; }
    public required string Scope { get; set; }
    public string? ActorToken { get; set; }
    public TokenType? ActorTokenType { get; set; }
    public string? ActorSubject { get; set; }
    public TokenType? RequestTokenType { get; set; }
    public string? Audience { get; set; }
}