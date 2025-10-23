namespace eShop.Auth.Api.Entities;

public class ConsentEntity : Entity
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid ClientId { get; set; }

    public ICollection<GrantedScopeEntity> GrantedScopes { get; set; } = null!;
    public UserEntity User { get; set; } = null!;
    public ClientEntity Client { get; set; } = null!;
}