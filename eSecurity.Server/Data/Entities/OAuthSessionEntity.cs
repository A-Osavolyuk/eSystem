using eSecurity.Core.Security.Authorization.OAuth;
using eSystem.Core.Data.Entities;

namespace eSecurity.Server.Data.Entities;

public class OAuthSessionEntity : Entity
{
    public Guid Id { get; set; }
    public Guid? LinkedAccountId { get; set; }
    
    public string Token { get; set; } = string.Empty;
    public OAuthSignType SignType { get; set; }
    public DateTimeOffset? ExpiredDate { get; set; }
    
    public UserLinkedAccountEntity? LinkedAccount { get; set; }
}