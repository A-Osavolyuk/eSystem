using eShop.Domain.Abstraction.Messaging;
using eShop.Domain.Common.Messaging;
using eShop.Domain.Enums;
using eShop.MessageBus.Bus;
using MassTransit;
using Newtonsoft.Json;

namespace eShop.MessageBus.Consumers;

public class MessageConsumer(
    MessageBusRegistry registry,
    IBus bus) : IConsumer<MessageRequest>
{
    private readonly MessageBusRegistry registry = registry;
    private readonly IBus bus = bus;

    public async Task Consume(ConsumeContext<MessageRequest> context)
    {
        var request = context.Message;
        var queueName = $"{request.Type.ToString().ToLower()}:{request.Queue}";
        var configuration = registry.GetByQueueName(queueName);

        if (configuration is null)
        {
            throw new NotSupportedException($"Queue {request.Queue} not supported");
        }

        var message = MapTo(configuration.MessageType, request.Payload, request.Credentials);
        await SendAsync(configuration.QueueName, message);
    }

    private async Task SendAsync(string queue, object message, CancellationToken cancellationToken = default)
    {
        var address = new Uri($"rabbitmq://localhost/{queue}");
        var endpoint = await bus.GetSendEndpoint(address);
        await endpoint.Send(message, cancellationToken);
    }

    private static object MapTo(Type type, Dictionary<string, string> payload, Dictionary<string, string> credentials)
    {
        var combined = payload.ToDictionary(kvp => kvp.Key, object (kvp) => kvp.Value);
        combined.Add("Credentials", credentials.ToDictionary(kvp => kvp.Key, object (kvp) => kvp.Value));
        
        var json = JsonConvert.SerializeObject(combined);
        return JsonConvert.DeserializeObject(json, type)!;
    }
}