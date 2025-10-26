using eSystem.Core.Common.Messaging;
using eSystem.SmsSender.Api.Interfaces;

namespace eSystem.SmsSender.Api.Consumers;

public class SmsConsumer(ISmsService smsService) : IConsumer<MessageRequest>
{
    private readonly ISmsService smsService = smsService;
    
    public async Task Consume(ConsumeContext<MessageRequest> context)
    {
        var message = context.Message;

        await smsService.SendMessageAsync(message.Credentials["To"], message.Body);
    }
}