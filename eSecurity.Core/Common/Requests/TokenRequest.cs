namespace eSecurity.Core.Common.Requests;

public class TokenRequest
{
    public required string GrantType { get; set; }
    public required string ClientId { get; set; }
    public string? RedirectUri { get; set; }
    public string? RefreshToken { get; set; }
    public string? Code { get; set; }
    public string? ClientSecret { get; set; }
    public string? CodeVerifier { get; set; }
    public string? Scope { get; set; }
}