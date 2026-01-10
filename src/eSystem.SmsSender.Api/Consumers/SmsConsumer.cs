using eSystem.Core.Common.Messaging;
using eSystem.SmsSender.Api.Interfaces;

namespace eSystem.SmsSender.Api.Consumers;

public class SmsConsumer(ISmsService smsService) : IConsumer<MessageRequest>
{
    private readonly ISmsService _smsService = smsService;
    
    public async Task Consume(ConsumeContext<MessageRequest> context)
    {
        var message = context.Message;

        await _smsService.SendMessageAsync(message.Credentials["To"], message.Body);
    }
}