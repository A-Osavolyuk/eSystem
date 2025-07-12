using eShop.Domain.Common.Messaging;
using MassTransit;
using Newtonsoft.Json;

namespace eShop.Auth.Api.Services;

[Injectable(typeof(IMessageService), ServiceLifetime.Scoped)]
public sealed class MessageService(IBus bus) : IMessageService
{
    private readonly IBus bus = bus;
    
    public async ValueTask SendMessageAsync(SenderType type, Message message, CancellationToken cancellationToken = default)
    {
        var body = message.Build();
        var credentials = message.Credentials;

        var queue = type switch
        {
            SenderType.Email => "email-message",
            SenderType.Sms => "sms-message",
            SenderType.Telegram => "telegram-message",
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
        
        var request = new MessageRequest()
        {
            Type = type,
            Queue = queue,
            Body = body,
            Credentials = credentials
        };

        var address = ToQueueUri("unified-message");
        var endpoint = await bus.GetSendEndpoint(address);
        await endpoint.Send(request as object, cancellationToken);
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