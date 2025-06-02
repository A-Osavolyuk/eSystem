namespace eShop.Auth.Api.Interfaces;

public interface IMessageService
{
    public ValueTask SendMessageAsync(MessageType type, string queueName, object? payload,
        MessageCredentials credentials, CancellationToken cancellationToken = default);
}