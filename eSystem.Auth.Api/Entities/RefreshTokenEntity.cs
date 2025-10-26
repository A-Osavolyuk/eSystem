using eSystem.Core.Data.Entities;

namespace eSystem.Auth.Api.Entities;

public class RefreshTokenEntity : Entity, IExpirable
{
    public Guid Id { get; init; }
    public Guid SessionId { get; set; }
    public Guid ClientId { get; set; }
    public string Token { get; set; } = string.Empty;
    public bool IsValid => ExpireDate > DateTimeOffset.UtcNow;
    
    public DateTimeOffset ExpireDate { get; set; }
    public DateTimeOffset? RefreshDate { get; set; }
    public SessionEntity Session { get; init; } = null!;
    public ClientEntity Client { get; init; } = null!;
}