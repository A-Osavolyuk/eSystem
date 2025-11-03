using eSystem.Core.Data.Entities;

namespace eSecurity.Data.Entities;

public class AuthorizationCodeEntity : Entity
{
    public Guid Id { get; set; }
    public Guid ClientId { get; set; }
    public Guid DeviceId { get; set; }
    
    public required string Code { get; set; }
    public required string Nonce { get; set; }
    public required string RedirectUri { get; set; }
    
    public string? CodeChallenge { get; set; }
    public string? CodeChallengeMethod { get; set; }
    
    public bool Used { get; set; }
    public DateTimeOffset ExpireDate { get; set; }

    public ClientEntity Client { get; set; } = null!;
    public UserDeviceEntity Device { get; set; } = null!;
}