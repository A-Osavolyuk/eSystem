namespace eShop.Auth.Api.Entities;

public class RecoveryCodeEntity : Entity
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    
    public string Code { get; set; } = string.Empty;
    
    public UserEntity User { get; set; } = null!;
}