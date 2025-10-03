namespace eShop.Auth.Api.Entities;

public class UserTwoFactorMethodEntity : Entity
{
    public Guid UserId { get; set; }
    public Guid ProviderId { get; set; }
    public bool IsPrimary { get; set; }
    
    public UserEntity User { get; set; } = null!;
    public TwoFactorMethodEntity Method { get; set; } = null!;
}