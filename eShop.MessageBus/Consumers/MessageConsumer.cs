using eShop.Domain.Common.Messaging;
using eShop.MessageBus.Bus;
using MassTransit;

namespace eShop.MessageBus.Consumers;

public class MessageConsumer(
    MessageBusRegistry registry,
    IBus bus) : IConsumer<MessageRequest>
{
    private readonly MessageBusRegistry registry = registry;
    private readonly IBus bus = bus;

    public async Task Consume(ConsumeContext<MessageRequest> context)
    {
        throw new NotImplementedException();
    }

    private async Task SendMessageAsync(string queue, object message, CancellationToken cancellationToken = default)
    {
        var address = new Uri($"rabbitmq://localhost/{queue}");
        var endpoint = await bus.GetSendEndpoint(address);
        await endpoint.Send(message as object, cancellationToken);
    }
}