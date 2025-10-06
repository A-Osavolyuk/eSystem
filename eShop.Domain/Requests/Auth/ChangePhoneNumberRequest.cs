namespace eShop.Domain.Requests.Auth;

public record ChangePhoneNumberRequest
{
    public required Guid UserId { get; set; }
    public required PhoneNumberType Type { get; set; }
    public required string NewPhoneNumber { get; set; }
}