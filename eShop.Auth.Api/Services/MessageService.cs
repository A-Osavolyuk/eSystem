using eShop.Domain.Messages.Email;
using MassTransit;

namespace eShop.Auth.Api.Services;

[Injectable(typeof(IMessageService), ServiceLifetime.Scoped)]
public sealed class MessageService(IBus bus) : IMessageService
{
    private readonly IBus bus = bus;
    
    private Uri CreateQueryUri(string queryName)
    {
        const string uriBase = "rabbitmq://localhost/";

        var uri = new Uri(
            new StringBuilder(uriBase)
                .Append(queryName)
                .ToString());

        return uri;
    }

    public async ValueTask SendMessageAsync(string queryName, EmailMessage message, CancellationToken cancellationToken = default)
    {
        var address = CreateQueryUri(queryName);
        var endpoint = await bus.GetSendEndpoint(address);
        await endpoint.Send(message as object, cancellationToken);
    }

    public async ValueTask SendMessageAsync(string queryName, SmsMessage message, CancellationToken cancellationToken = default)
    {
        var address = CreateQueryUri(queryName);
        var endpoint = await bus.GetSendEndpoint(address);
        await endpoint.Send(message as object, cancellationToken);
    }
}