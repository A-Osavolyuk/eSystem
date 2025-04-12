using eShop.Domain.Requests.API.Sms;
using eShop.Domain.Responses.API.Sms;

namespace eShop.SmsSender.Api.Interfaces;

public interface ISmsService
{
    public Task<SingleMessageResponse> SendSingleMessage(SingleMessageRequest request);
}