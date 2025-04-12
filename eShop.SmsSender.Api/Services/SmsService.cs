using eShop.Domain.Requests.API.Sms;
using eShop.Domain.Responses.API.Sms;
using eShop.SmsSender.Api.Interfaces;

namespace eShop.SmsSender.Api.Services;

public class SmsService(IAmazonSimpleNotificationService simpleNotificationService) : ISmsService
{
    private readonly IAmazonSimpleNotificationService simpleNotificationService = simpleNotificationService;

    public async Task<SingleMessageResponse> SendSingleMessage(SingleMessageRequest request)
    {
        var response = await simpleNotificationService.PublishAsync(new PublishRequest
        {
            Message = request.Message,
            PhoneNumber = request.PhoneNumber
        });

        if (response.HttpStatusCode == HttpStatusCode.OK)
        {
            return new SingleMessageResponse()
            {
                Message = "Successfully sent SMS",
                IsSucceeded = true,
                StatusCode = response.HttpStatusCode
            };
        }
        else
        {
            return new SingleMessageResponse()
            {
                Message = $"Failed to send SMS with code: {response.HttpStatusCode}",
                IsSucceeded = false,
                StatusCode = response.HttpStatusCode
            };
        }
    }
}