namespace eShop.Auth.Api.Interfaces;

public interface IMessageService
{
    public ValueTask SendMessageAsync(SenderType type, string queueName, object? payload,
        MessageCredentials credentials, CancellationToken cancellationToken = default);
}