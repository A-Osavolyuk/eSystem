using eShop.Domain.Requests.API.Sms;
using eShop.SmsSender.Api.Interfaces;

namespace eShop.SmsSender.Api.Consumers;

public class VerifyPhoneNumberConsumer(ISmsService smsService) : IConsumer<VerifyPhoneNumberMessage>
{
    private readonly ISmsService smsService = smsService;

    public async Task Consume(ConsumeContext<VerifyPhoneNumberMessage> context)
    {
        var request = context.Message;
        var response = await smsService.SendSingleMessage(new SingleMessageRequest()
        {
            Message = $"Phone number verification code: {request.Code}",
            PhoneNumber = request.PhoneNumber
        });
        await context.RespondAsync(response);
    }
}