namespace eSystem.Core.Requests.Auth;

public record RefreshRequest
{
    public string RefreshToken { get; set; } = string.Empty;
}