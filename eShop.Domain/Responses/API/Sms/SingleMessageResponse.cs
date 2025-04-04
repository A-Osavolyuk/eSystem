using eShop.Domain.Abstraction.Responses;

namespace eShop.Domain.Responses.Api.Sms;

public class SingleMessageResponse : ResponseBase
{
    public bool IsSucceeded { get; set; }
    public HttpStatusCode StatusCode { get; set; }
}