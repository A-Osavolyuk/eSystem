namespace eShop.Domain.Requests.API.Auth;

public class ConfirmResetPhoneNumberRequest
{
    public Guid Id { get; set; }
    public string NewPhoneNumber { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
}