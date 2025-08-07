namespace eShop.Auth.Api.Entities;

public class LoginSessionEntity : Entity
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }

    public LoginStatus Status { get; set; }
    public LoginType Type { get; set; }
    
    public string? Provider { get; set; }
    public string IpAddress { get; set; } = string.Empty;
    public string UserAgent { get; set; } =  string.Empty;
    public string? Device { get; set; }
    public string? Location { get; set; }
    
    public DateTimeOffset Timestamp { get; set; }

    public UserEntity User { get; set; } = null!;
}