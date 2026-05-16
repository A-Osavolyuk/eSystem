using eSystem.Core.Server.Messaging;
using eSystem.MessageBus.Configurations;

namespace eSystem.MessageBus.Interfaces;

public interface IMessageBusBuilder
{
    public IMessageBusBuilder AddQueue<TMessage>(string queueName, SenderType sender);
    public IReadOnlyList<QueueConfiguration> Build();
}