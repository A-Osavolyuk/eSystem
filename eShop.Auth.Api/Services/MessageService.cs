using MassTransit;

namespace eShop.Auth.Api.Services;

public class MessageService(IBus bus) : IMessageService
{
    private readonly IBus bus = bus;

    public async ValueTask SendMessageAsync(string queryName, EmailMessage message, CancellationToken cancellationToken = default)
    {
        var address = CreateQueryUri(queryName);
        var endpoint = await bus.GetSendEndpoint(address);
        await endpoint.Send(message, cancellationToken);
    }

    public async ValueTask SendMessageAsync(string queryName, SmsMessage message, CancellationToken cancellationToken = default)
    {
        var address = CreateQueryUri(queryName);
        var endpoint = await bus.GetSendEndpoint(address);
        await endpoint.Send(message, cancellationToken);
    }

    private Uri CreateQueryUri(string queryName)
    {
        const string uriBase = "rabbitmq://localhost/";

        var uri = new Uri(
            new StringBuilder(uriBase)
                .Append(queryName)
                .ToString());

        return uri;
    }
}