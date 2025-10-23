namespace eSystem.Domain.Requests.Auth;

public class LoadOAuthSessionRequest
{
    public Guid Id { get; set; }
    public string Token { get; set; } = string.Empty;
}