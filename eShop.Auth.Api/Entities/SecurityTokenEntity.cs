using eShop.Domain.Abstraction.Data;

namespace eShop.Auth.Api.Entities;

public class SecurityTokenEntity : IIdentifiable<Guid>, IAuditable, IExpireable
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public string Token { get; init; } = string.Empty;
    public DateTime ExpireDate { get; set; }
    public DateTime CreateDate { get; set; }
    public DateTime UpdateDate { get; set; }
    public AppUser User { get; init; } = null!;
}