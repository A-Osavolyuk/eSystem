using eSystem.Domain.Common.Messaging;
using eSystem.Domain.Enums;
using MassTransit;

namespace eSystem.MessageBus.Consumers;

public class MessageConsumer(IBus bus) : IConsumer<MessageRequest>
{
    private readonly IBus bus = bus;

    public async Task Consume(ConsumeContext<MessageRequest> context)
    {
        var request = context.Message;

        var queue = request.Type switch
        {
            SenderType.Email => "email-message",
            SenderType.Sms => "sms-message",
            SenderType.Telegram => "telegram-message",
            _ => throw new ArgumentOutOfRangeException()
        };

        var address = new Uri($"rabbitmq://localhost/{queue}");
        var endpoint = await bus.GetSendEndpoint(address);
        await endpoint.Send(request);
    }
}