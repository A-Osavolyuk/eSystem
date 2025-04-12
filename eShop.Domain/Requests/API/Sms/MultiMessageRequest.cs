namespace eShop.Domain.Requests.API.Sms;

public class MultiMessageRequest
{
    public string Message { get; set; } = string.Empty;
    public List<string> PhoneNumbers { get; set; } = new();
}

