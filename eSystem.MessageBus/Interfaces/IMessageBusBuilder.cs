using eSystem.Domain.Common.Messaging;
using eSystem.Domain.Enums;
using eSystem.MessageBus.Configurations;

namespace eSystem.MessageBus.Interfaces;

public interface IMessageBusBuilder
{
    public IMessageBusBuilder AddQueue<TMessage>(string queueName, SenderType sender);
    public IReadOnlyList<QueueConfiguration> Build();
}