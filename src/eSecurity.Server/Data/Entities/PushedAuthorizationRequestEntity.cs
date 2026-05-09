using eSystem.Core.Data.Entities;
using eSystem.Core.Security.Authentication.OpenIdConnect;

namespace eSecurity.Server.Data.Entities;

public sealed class PushedAuthorizationRequestEntity : Entity
{
    public Guid Id { get; set; }
    
    public required ResponseType ResponseType { get; set; }
    public required string ClientId { get; set; }
    public required string RedirectUri { get; set; }
    public string? Nonce { get; set; }
    public string? State { get; set; }
    public string? CodeChallenge { get; set; }
    public ChallengeMethod? CodeChallengeMethod { get; set; }

    public ICollection<PushedAuthorizationRequestScopeEntity> Scopes { get; set; } = [];
    public ICollection<PushedAuthorizationRequestPromptEntity> Prompts { get; set; } = [];
}