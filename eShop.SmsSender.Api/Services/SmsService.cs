using eShop.Domain.Common.Results;
using eShop.SmsSender.Api.Interfaces;

namespace eShop.SmsSender.Api.Services;

public class SmsService(IAmazonSimpleNotificationService simpleNotificationService) : ISmsService
{
    private readonly IAmazonSimpleNotificationService simpleNotificationService = simpleNotificationService;

    public async Task<Result> SendMessageAsync(string phoneNumber, string message)
    {
        var response = await simpleNotificationService.PublishAsync(new PublishRequest
        {
            Message = message,
            PhoneNumber = phoneNumber
        });

        if (response.HttpStatusCode != HttpStatusCode.OK)
        {
            return Results.InternalServerError($"Failed to send SMS with code: {response.HttpStatusCode}");
        }

        return Result.Success();
    }
}