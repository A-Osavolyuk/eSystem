using eSecurity.Idp.Security.Authorization.Authorize;
using eSystem.Core.Security.Authentication.OpenIdConnect;

namespace eSecurity.Idp.Security.Authorization.Prompt;

public sealed class PromptContext
{
    public required AuthorizationFlow AuthorizationFlow { get; init; }
    public required ResponseType ResponseType { get; set; }
    public required Guid ClientId { get; set; }
    public required string RedirectUri { get; set; }
    public required List<string> Scopes { get; set; }
    public required List<PromptType> Prompts { get; set; }
    public string? RequestUri { get; set; }
    public string? Request { get; set; }
    public string? State { get; set; }
    public string? Nonce { get; set; }
    public string? CodeChallenge { get; set; }
    public ChallengeMethod? CodeChallengeMethod { get; set; }
}