namespace eShop.Domain.Responses.Api.Sms;

public class MultiMessageResponse : ResponseBase
{
    public bool IsSucceeded { get; set; }
    public HttpStatusCode StatusCode { get; set; }
}