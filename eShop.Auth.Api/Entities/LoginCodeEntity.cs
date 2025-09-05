namespace eShop.Auth.Api.Entities;

public class LoginCodeEntity : Entity, IExpirable
{
    public Guid Id { get; init; }
    public Guid UserId { get; set; }
    public Guid ProviderId { get; set; }
    
    public string CodeHash { get; set; } = string.Empty;
    public DateTimeOffset ExpireDate { get; set; }
    public TwoFactorProviderEntity TwoFactorProvider { get; set; } = null!;
    public UserEntity User { get; set; } = null!;
}