namespace eShop.Auth.Api.Entities;

public class LoginTokenEntity : IEntity<Guid>, IExpireable
{
    public Guid Id { get; init; }
    public Guid UserId { get; set; }
    public Guid ProviderId { get; set; }
    
    public string Token { get; set; } = string.Empty;
    public DateTime ExpireDate { get; set; }
    
    public DateTime? CreateDate { get; set; }
    public DateTime? UpdateDate { get; set; }

    public ProviderEntity Provider { get; set; } = null!;
    public UserEntity User { get; set; } = null!;
}