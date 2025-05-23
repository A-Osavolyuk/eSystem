namespace eShop.Auth.Api.Entities;

public class UserSecretEntity : IEntity<Guid>
{
    public Guid Id { get; init; }
    public Guid UserId { get; set; }

    public string Secret { get; set; } = string.Empty;
    
    public DateTimeOffset? CreateDate { get; set; }
    public DateTimeOffset? UpdateDate { get; set; }
    
    public UserEntity User { get; set; } = null!;
}