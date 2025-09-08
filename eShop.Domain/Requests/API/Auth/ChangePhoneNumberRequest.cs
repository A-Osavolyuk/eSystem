namespace eShop.Domain.Requests.API.Auth;

public record ChangePhoneNumberRequest
{
    public required Guid UserId { get; set; }
    public required string CurrentPhoneNumber { get; set; }
    public required string NewPhoneNumber { get; set; }
}