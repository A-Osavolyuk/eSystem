using eSystem.Core.Security.Authentication.OpenIdConnect.Constants;

namespace eSecurity.Client.Security.Authentication.OpenIdConnect.Authorization;

public sealed class AuthorizationContext
{
    public required ResponseType ResponseType { get; set; }
    public required string ClientId { get; set; }
    public required string RedirectUri { get; set; }
    public required string State { get; set; }
    public required string Nonce { get; set; }
    public required string Scope { get; set; }
    public List<PromptType> Prompts { get; set; } = [];
    public string? CodeChallenge { get; set; }
    public ChallengeMethod? CodeChallengeMethod { get; set; }
    public string? ReturnUrl { get; set; }
}