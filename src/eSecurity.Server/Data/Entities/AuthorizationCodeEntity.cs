using eSecurity.Server.Security.Authorization.OAuth.Protocol;
using eSystem.Core.Data.Entities;
using eSystem.Core.Security.Authentication.OpenIdConnect.Constants;

namespace eSecurity.Server.Data.Entities;

public class AuthorizationCodeEntity : Entity
{
    public Guid Id { get; set; }
    
    public required AuthorizationProtocol Protocol { get; set; }
    
    public required string Code { get; set; }
    public required string Nonce { get; set; }
    public required string RedirectUri { get; set; }
    
    public string? CodeChallenge { get; set; }
    public ChallengeMethod? CodeChallengeMethod { get; set; }
    
    public bool Used { get; set; }
    public DateTimeOffset ExpiredAt { get; set; }

    public Guid ClientId { get; set; }
    public ClientEntity Client { get; set; } = null!;
    
    public Guid UserId { get; set; }
    public UserEntity User { get; set; } = null!;
}