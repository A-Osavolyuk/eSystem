namespace eShop.Domain.Requests.API.Auth;

public record ChangePhoneNumberRequest
{
    public Guid UserId { get; set; }
    public string NewPhoneNumber { get; set; } = string.Empty;
}