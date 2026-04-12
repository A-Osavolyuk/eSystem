using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;
using eSystem.SmsSender.Api.Interfaces;

namespace eSystem.SmsSender.Api.Services;

public class SmsService(IAmazonSimpleNotificationService simpleNotificationService) : ISmsService
{
    private readonly IAmazonSimpleNotificationService _simpleNotificationService = simpleNotificationService;

    public async Task<Result> SendMessageAsync(string phoneNumber, string message)
    {
        var response = await _simpleNotificationService.PublishAsync(new PublishRequest
        {
            Message = message,
            PhoneNumber = phoneNumber
        });

        if (response.HttpStatusCode != HttpStatusCode.OK)
        {
            return Results.ServerError(ServerErrorCode.InternalServerError, new Error()
            {
                Code = ErrorCode.ServerError,
                Description = $"Failed to send SMS with code: {response.HttpStatusCode}"
            });
        }

        return Results.Success(SuccessCodes.Ok);
    }
}