namespace eShop.Auth.Api.Entities;

public class UserSecretEntity : IEntity<Guid>
{
    public Guid Id { get; init; }
    public Guid UserId { get; set; }

    public string Secret { get; set; } = string.Empty;
    
    public DateTime? CreateDate { get; set; }
    public DateTime? UpdateDate { get; set; }
    
    public UserEntity User { get; set; } = null!;
}