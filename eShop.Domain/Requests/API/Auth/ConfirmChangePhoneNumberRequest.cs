namespace eShop.Domain.Requests.Api.Auth;

public record class ConfirmChangePhoneNumberRequest
{
    public string CurrentPhoneNumber { get; set; } = string.Empty;
    public string NewPhoneNumber { get; set; } = string.Empty;
    public CodeSet CodeSet { get; set; } = null!;
}