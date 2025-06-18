namespace eShop.Auth.Api.Entities;

public class RefreshTokenEntity : IEntity, IExpireable
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public string Token { get; set; } = string.Empty;
    
    public DateTimeOffset ExpireDate { get; set; }
    public DateTimeOffset? CreateDate { get; set; }
    public DateTimeOffset? UpdateDate { get; set; }
    
    public UserEntity UserEntity { get; init; } = null!;
}