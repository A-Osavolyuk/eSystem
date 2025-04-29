namespace eShop.Domain.Requests.API.Auth;

public record ConfirmChangePhoneNumberRequest
{
    public string CurrentPhoneNumber { get; set; } = string.Empty;
    public string NewPhoneNumber { get; set; } = string.Empty;
    public CodeSet CodeSet { get; set; } = null!;
}