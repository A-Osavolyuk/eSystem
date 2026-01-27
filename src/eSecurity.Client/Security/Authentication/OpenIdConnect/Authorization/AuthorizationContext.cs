namespace eSecurity.Client.Security.Authentication.OpenIdConnect.Authorization;

public sealed class AuthorizationContext
{
    public required string ResponseType { get; set; }
    public required string ClientId { get; set; }
    public required string RedirectUri { get; set; }
    public required string State { get; set; }
    public required string Nonce { get; set; }
    public required string Scope { get; set; }
    public List<string> Prompts { get; set; } = [];
    public string? CodeChallenge { get; set; }
    public string? CodeChallengeMethod { get; set; }
    public string? ReturnUrl { get; set; }
}