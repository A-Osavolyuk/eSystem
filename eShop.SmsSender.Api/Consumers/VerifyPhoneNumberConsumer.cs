using eShop.Domain.Requests.API.Sms;
using eShop.SmsSender.Api.Interfaces;

namespace eShop.SmsSender.Api.Consumers;

public class VerifyPhoneNumberConsumer(ISmsService smsService) : IConsumer<VerifyPhoneNumberSmsMessage>
{
    private readonly ISmsService smsService = smsService;

    public async Task Consume(ConsumeContext<VerifyPhoneNumberSmsMessage> context)
    {
        var request = context.Message;
        var response = await smsService.SendSingleMessage(new SingleMessageRequest()
        {
            Message = $"Phone number verification code: {request.Code}",
            PhoneNumber = request.Credentials.PhoneNumber
        });
        await context.RespondAsync(response);
    }
}