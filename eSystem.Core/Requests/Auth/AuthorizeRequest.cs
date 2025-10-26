namespace eSystem.Core.Requests.Auth;

public class AuthorizeRequest
{
    public Guid UserId { get; set; }
    public string ClientId { get; set; } = string.Empty;
    public string RedirectUri { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string Nonce { get; set; } = string.Empty;
    public List<string> Scopes { get; set; } = [];
}