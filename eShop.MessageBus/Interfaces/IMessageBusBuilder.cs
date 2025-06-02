using eShop.Domain.Enums;
using eShop.MessageBus.Builders;
using eShop.MessageBus.Configurations;

namespace eShop.MessageBus.Interfaces;

public interface IMessageBusBuilder
{
    public IMessageBusBuilder AddQueue<TMessage>(string queueName, SenderType sender);
    public IReadOnlyList<QueueConfiguration> Build();
}