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
    
    private Uri ToQueueUri(string queueName) => new($"rabbitmq://localhost/{queueName}");
}