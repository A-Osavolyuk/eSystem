using eSystem.Core.Data.Entities;
using OAuthFlow = eSecurity.Core.Security.Authorization.OAuth.OAuthFlow;

namespace eSecurity.Server.Data.Entities;

public class OAuthSessionEntity : Entity
{
    public Guid Id { get; set; }
    public Guid? LinkedAccountId { get; set; }
    
    public string Token { get; set; } = string.Empty;
    public OAuthFlow Flow { get; set; }
    public DateTimeOffset? ExpiredDate { get; set; }
    
    public UserLinkedAccountEntity? LinkedAccount { get; set; }
}