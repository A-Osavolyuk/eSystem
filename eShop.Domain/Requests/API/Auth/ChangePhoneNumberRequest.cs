namespace eShop.Domain.Requests.Api.Auth;

public record class ChangePhoneNumberRequest
{
    public string CurrentPhoneNumber { get; set; } = string.Empty;
    public string NewPhoneNumber { get; set; } = string.Empty;
}