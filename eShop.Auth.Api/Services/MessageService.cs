using eShop.Domain.Abstraction.Messaging;

namespace eShop.Auth.Api.Services;

public class MessageService(IBus bus) : IMessageService
{
    private readonly IBus bus = bus;

    public async ValueTask SendMessageAsync(string queryName, EmailMessage message)
    {
        var address = CreateQueryUri(queryName);
        var endpoint = await bus.GetSendEndpoint(address);
        await endpoint.Send(message);
    }

    public async ValueTask SendMessageAsync(string queryName, SmsMessage message)
    {
        var address = CreateQueryUri(queryName);
        var endpoint = await bus.GetSendEndpoint(address);
        await endpoint.Send(message);
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