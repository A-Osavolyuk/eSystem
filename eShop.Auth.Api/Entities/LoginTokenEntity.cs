namespace eShop.Auth.Api.Entities;

public class LoginTokenEntity : IEntity, IExpireable
{
    public Guid Id { get; init; }
    public Guid UserId { get; set; }
    public Guid ProviderId { get; set; }
    
    public string Token { get; set; } = string.Empty;
    public DateTimeOffset ExpireDate { get; set; }
    
    public DateTimeOffset? CreateDate { get; set; }
    public DateTimeOffset? UpdateDate { get; set; }

    public ProviderEntity Provider { get; set; } = null!;
    public UserEntity User { get; set; } = null!;
}