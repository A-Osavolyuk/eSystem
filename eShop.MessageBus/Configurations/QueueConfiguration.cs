using eShop.Domain.Enums;

namespace eShop.MessageBus.Configurations;

public class QueueConfiguration(Type type, string queueName, SenderType sender)
{
    public Type MessageType { get; } = type;
    public string QueueName { get; } = queueName;
    public SenderType Sender { get; } = sender;
}