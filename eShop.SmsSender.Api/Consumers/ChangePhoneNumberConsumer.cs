using eShop.Domain.Requests.API.Sms;
using eShop.SmsSender.Api.Interfaces;

namespace eShop.SmsSender.Api.Consumers;

public class ChangePhoneNumberConsumer(
    ISmsService smsService) : IConsumer<ChangePhoneNumberMessage>
{
    private readonly ISmsService smsService = smsService;

    public async Task Consume(ConsumeContext<ChangePhoneNumberMessage> context)
    {
        var request = context.Message;
        var response = await smsService.SendSingleMessage(new SingleMessageRequest()
        {
            Message = $"Phone number change code: {request.Code}",
            PhoneNumber = request.Code
        });
        await context.RespondAsync(response);
    }
}