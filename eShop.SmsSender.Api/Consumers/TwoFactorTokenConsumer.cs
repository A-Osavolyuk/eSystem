using eShop.Domain.Requests.API.Sms;
using eShop.SmsSender.Api.Interfaces;

namespace eShop.SmsSender.Api.Consumers;

public class TwoFactorTokenConsumer(ISmsService smsService) : IConsumer<TwoFactorTokenSmsMessage>
{
    private readonly ISmsService smsService = smsService;

    public async Task Consume(ConsumeContext<TwoFactorTokenSmsMessage> context)
    {
        var request = context.Message;
        var response = await smsService.SendSingleMessage(new SingleMessageRequest()
        {
            Message = $"Two-factor authentication code: {request.Token}",
            PhoneNumber = request.PhoneNumber
        });
        await context.RespondAsync(response);
    }
}