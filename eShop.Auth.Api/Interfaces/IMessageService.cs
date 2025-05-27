namespace eShop.Auth.Api.Interfaces;

public interface IMessageService
{
    public ValueTask SendMessageAsync(string queryName, EmailMessage message, CancellationToken cancellationToken = default);
    public ValueTask SendMessageAsync(string queryName, SmsMessage message, CancellationToken cancellationToken = default);
}