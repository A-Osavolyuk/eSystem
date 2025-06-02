using eShop.Domain.Enums;
using eShop.MessageBus.Configurations;
using eShop.MessageBus.Interfaces;

namespace eShop.MessageBus.Builders;

public class MessageBusBuilder : IMessageBusBuilder
{
    private readonly List<QueueConfiguration> queues = new();

    public IMessageBusBuilder AddQueue<TMessage>(string queueName, SenderType sender)
    {
        queues.Add(new QueueConfiguration(typeof(TMessage), $"{sender.ToString().ToLower()}:{queueName}",sender));
        return this;
    }

    public IReadOnlyList<QueueConfiguration> Build() => queues.AsReadOnly();
}