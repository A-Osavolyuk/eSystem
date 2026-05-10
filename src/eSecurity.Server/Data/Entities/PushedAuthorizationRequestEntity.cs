using eSecurity.Server.Security.Authorization.Par;
using eSystem.Core.Data.Entities;
using eSystem.Core.Security.Authentication.OpenIdConnect;

namespace eSecurity.Server.Data.Entities;

public sealed class PushedAuthorizationRequestEntity : Entity
{
    public Guid Id { get; init; }
    public required string RequestUri { get; set; }
    public required ResponseType ResponseType { get; init; }
    public required string RedirectUri { get; init; }
    public string? Nonce { get; init; }
    public string? State { get; init; }
    public string? CodeChallenge { get; init; }
    public ChallengeMethod? CodeChallengeMethod { get; init; }

    public ParState Status { get; init; }
    public DateTimeOffset ExpiredAt { get; init; }
    public DateTimeOffset? ConsumedAt { get; set; }
    public DateTimeOffset? CancelledAt { get; set; }
    
    public Guid ClientId { get; init; }
    public ClientEntity Client { get; init; } = null!;

    public ICollection<PushedAuthorizationRequestScopeEntity> Scopes { get; set; } = null!;
    public ICollection<PushedAuthorizationRequestPromptEntity> Prompts { get; set; } = null!;

    public void AddPrompts(IEnumerable<PromptType> prompts)
    {
        Prompts = [];
        foreach (var prompt in prompts)
        {
            Prompts.Add(new PushedAuthorizationRequestPromptEntity()
            {
                Id = Guid.CreateVersion7(),
                RequestId = Id,
                Prompt = prompt,
            });
        }
    }

    public void AddScopes(IEnumerable<string> scopes)
    {
        Scopes = [];
        foreach (var scope in scopes)
        {
            Scopes.Add(new PushedAuthorizationRequestScopeEntity()
            {
                Id = Guid.CreateVersion7(),
                RequestId = Id,
                Scope = scope
            });
        }
    }
}