namespace eSystem.Core.Requests.Auth;

public class TokenRequest
{
    public Guid UserId { get; set; }
    public required string GrantType { get; set; }
    public required string Code { get; set; }
    public required string RedirectUri { get; set; }
    public required string ClientId { get; set; }
    public string? ClientSecret { get; set; }
    public string? CodeVerifier { get; set; }
}