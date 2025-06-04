namespace eShop.Domain.Requests.API.Auth;

public record ConfirmPhoneNumberChangeRequest
{
    public string CurrentPhoneNumber { get; set; } = string.Empty;
    public string NewPhoneNumber { get; set; } = string.Empty;
    public string CurrentPhoneNumberCode { get; set; } = string.Empty;
    public string NewPhoneNumberCode { get; set; } = string.Empty;
}