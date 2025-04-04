using eShop.Domain.Abstraction.Messaging;

namespace eShop.Auth.Api.Interfaces;

public interface IMessageService
{
    public ValueTask SendMessageAsync(string queryName, EmailMessage message);
    public ValueTask SendMessageAsync(string queryName, SmsMessage message);
}