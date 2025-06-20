namespace eShop.Domain.Requests.API.Auth;

public class AddPhoneNumberRequest
{
    public Guid Id { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
}