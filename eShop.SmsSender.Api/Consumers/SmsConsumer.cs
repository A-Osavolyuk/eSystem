using eShop.Domain.Common.Messaging;
using eShop.SmsSender.Api.Interfaces;

namespace eShop.SmsSender.Api.Consumers;

public class SmsConsumer(ISmsService smsService) : IConsumer<MessageRequest>
{
    private readonly ISmsService smsService = smsService;
    
    public async Task Consume(ConsumeContext<MessageRequest> context)
    {
        var message = context.Message;

        await smsService.SendMessageAsync(message.Credentials["PhoneNumber"], message.Body);
    }
}