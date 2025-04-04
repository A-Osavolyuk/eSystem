namespace eShop.Domain.Requests.Api.Sms;

public class SingleMessageRequest
{
    public string Message { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
}