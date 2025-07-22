namespace eShop.Domain.Requests.API.Auth;

public record ConfirmChangePhoneNumberRequest
{
    public Guid UserId { get; set; }
    public string NewPhoneNumber { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
}