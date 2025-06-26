namespace eShop.Domain.Responses.API.Sms;

public class SingleMessageResponse
{
    public bool IsSucceeded { get; set; }
    public HttpStatusCode StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
}