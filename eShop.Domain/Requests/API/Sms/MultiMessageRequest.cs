namespace eShop.Domain.Requests.Api.Sms;

public class MultiMessageRequest
{
    public string Message { get; set; } = string.Empty;
    public List<string> PhoneNumbers { get; set; } = new List<string>();
}

