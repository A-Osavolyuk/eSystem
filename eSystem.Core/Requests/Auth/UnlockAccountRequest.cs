namespace eSystem.Core.Requests.Auth;

public class UnlockAccountRequest
{
    public Guid UserId { get; set; }
    public string Code { get; set; } = string.Empty;
}