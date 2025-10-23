namespace eSystem.Auth.Api.Entities;

public class AuthorizationCodeEntity : Entity
{
    public Guid Id { get; set; }
    public Guid ClientId { get; set; }
    public Guid DeviceId { get; set; }
    
    public required string RedirectUri { get; set; }
    public required string CodeChallenge { get; set; }
    public required string CodeChallengeMethod { get; set; }
    public bool Used { get; set; }
    public DateTimeOffset ExpireDate { get; set; }

    public ClientEntity Client { get; set; } = null!;
    public UserDeviceEntity Device { get; set; } = null!;
}