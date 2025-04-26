using eShop.Domain.Interfaces;

namespace eShop.Auth.Api.Entities;

public class SecurityTokenEntity : IEntity<Guid>, IExpireable
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public string Token { get; init; } = string.Empty;
    public DateTime ExpireDate { get; set; }
    public DateTime CreateDate { get; set; }
    public DateTime UpdateDate { get; set; }
    public UserEntity UserEntity { get; init; } = null!;
}