namespace eShop.Auth.Api.Entities;

public class LoginCodeEntity : Entity, IExpireable
{
    public Guid Id { get; init; }
    public Guid UserId { get; set; }
    public Guid ProviderId { get; set; }
    
    public string Code { get; set; } = string.Empty;
    public DateTimeOffset ExpireDate { get; set; }
    public ProviderEntity Provider { get; set; } = null!;
    public UserEntity User { get; set; } = null!;
}