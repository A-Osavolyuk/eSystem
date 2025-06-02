using eShop.MessageBus.Configurations;

namespace eShop.MessageBus.Bus;

public class MessageBusRegistry(IReadOnlyList<QueueConfiguration> configs)
{
    public QueueConfiguration? GetByQueueName(string queue) =>
        configs.FirstOrDefault(c => c.QueueName == queue);

    public QueueConfiguration? GetByMessageType(Type type) =>
        configs.FirstOrDefault(c => c.MessageType == type);

    public IEnumerable<QueueConfiguration> GetAll() => configs;
}
