using eShop.Domain.Common.Messaging;
using MassTransit;
using Newtonsoft.Json;

namespace eShop.Auth.Api.Services;

[Injectable(typeof(IMessageService), ServiceLifetime.Scoped)]
public sealed class MessageService(IBus bus) : IMessageService
{
    private readonly IBus bus = bus;

    public async ValueTask SendMessageAsync(SenderType type, string queueName, object? payload,
        MessageCredentials credentials, CancellationToken cancellationToken = default)
    {
        var message = new MessageRequest()
        {
            Type = type,
            Queue = queueName,
            Payload = payload is null ? [] : ToDictionary(payload),
            Credentials = ToDictionary(credentials)
        };

        var address = ToQueueUri("unified-message");
        var endpoint = await bus.GetSendEndpoint(address);
        await endpoint.Send(message as object, cancellationToken);
    }

    private Uri ToQueueUri(string queueName) => new($"rabbitmq://localhost/{queueName}");

    private Dictionary<string, string> ToDictionary(object obj)
    {
        if (obj is null)
        {
            throw new NullReferenceException("Cannot serialize null to dictionary");
        }

        var json = JsonConvert.SerializeObject(obj);
        var dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(json)!;

        return dictionary;
    }
}