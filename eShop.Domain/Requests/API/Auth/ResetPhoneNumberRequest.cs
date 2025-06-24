namespace eShop.Domain.Requests.API.Auth;

public class ResetPhoneNumberRequest
{
    public Guid Id { get; set; }
    public string NewPhoneNumber { get; set; } = string.Empty;
}