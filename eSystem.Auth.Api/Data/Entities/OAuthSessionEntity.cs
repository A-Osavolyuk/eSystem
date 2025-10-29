using eSystem.Core.Data.Entities;
using eSystem.Core.Security.Authorization.OAuth;

namespace eSystem.Auth.Api.Data.Entities;

public class OAuthSessionEntity : Entity
{
    public Guid Id { get; set; }
    public Guid? LinkedAccountId { get; set; }
    
    public string Token { get; set; } = string.Empty;
    public OAuthSignType SignType { get; set; }
    public DateTimeOffset? ExpiredDate { get; set; }
    
    public UserLinkedAccountEntity? LinkedAccount { get; set; }
}