using eSystem.Core.Security.Authentication.OpenIdConnect;

namespace eSecurity.Idp.Security.Authorization.Authorize.Par;

public sealed class PushedAuthorizationRequest
{
    public ResponseType ResponseType { get; set; }
    public required string ClientId { get; set; }
    public required string Scope { get; set; }
    public string? RedirectUri { get; set; }
    public string? Nonce { get; set; }
    public string? State { get; set; }
    public string? Prompt { get; set; }
    public string? CodeChallenge { get; set; }
    public ChallengeMethod? CodeChallengeMethod { get; set; }
}