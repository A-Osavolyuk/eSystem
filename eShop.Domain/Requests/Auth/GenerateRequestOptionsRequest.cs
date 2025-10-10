namespace eShop.Domain.Requests.Auth;

public class GenerateRequestOptionsRequest
{
    public Guid UserId { get; set; }
    public string Username { get; set; } = string.Empty;
}