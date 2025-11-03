using eSystem.Core.Common.Messaging;
using MassTransit;

namespace eSecurity.Messaging;

public sealed class MessageService(IBus bus) : IMessageService
{
    private readonly IBus bus = bus;
    
    public async ValueTask SendMessageAsync(SenderType type, Message message, CancellationToken cancellationToken = default)
    {
        var body = message.Build();
        var credentials = message.Credentials;
        
        var request = new MessageRequest()
        {
            Type = type,
            Body = body,
            Credentials = credentials
        };

        var address = new Uri($"rabbitmq://localhost/unified-message");
        var endpoint = await bus.GetSendEndpoint(address);
        await endpoint.Send(request as object, cancellationToken);
    }
}