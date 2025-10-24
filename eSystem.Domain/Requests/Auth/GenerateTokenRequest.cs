namespace eSystem.Domain.Requests.Auth;

public class GenerateTokenRequest
{
    public Guid UserId { get; set; }
    public required string GrantType { get; set; }
    public required string ClientId { get; set; }
    public required string ClientSecret { get; set; }
    public required string Nonce { get; set; }
    public required string State { get; set; }
    public List<string> Scopes { get; set; } = [];
}