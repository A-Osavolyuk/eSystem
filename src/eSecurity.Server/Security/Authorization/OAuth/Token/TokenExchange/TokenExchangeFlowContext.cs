namespace eSecurity.Server.Security.Authorization.OAuth.Token.TokenExchange;

public sealed class TokenExchangeFlowContext
{
    public required string ClientId { get; set; }
    public required string GrantType { get; set; }
    public required string SubjectToken { get; set; }
    public required string SubjectTokenType { get; set; }
    public string? ActorToken { get; set; }
    public string? ActorTokenType { get; set; }
    public string? ActorSubject { get; set; }
    public string? RequestTokenType { get; set; }
    public string? Scope { get; set; }
    public string? Audience { get; set; }
}