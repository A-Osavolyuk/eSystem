using eShop.Domain.Enums;
using eShop.MessageBus.Configurations;

namespace eShop.MessageBus.Builders;

public class MessageBusBuilder
{
    private readonly List<QueueConfiguration> queues = new();

    public MessageBusBuilder AddQueue<TMessage>(string queueName, SenderType sender)
    {
        queues.Add(new QueueConfiguration(typeof(TMessage), queueName, sender));
        return this;
    }

    public IReadOnlyList<QueueConfiguration> Build() => queues.AsReadOnly();
}