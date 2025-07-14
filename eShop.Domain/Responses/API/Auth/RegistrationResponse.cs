namespace eShop.Domain.Responses.API.Auth;

public class RegistrationResponse
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}