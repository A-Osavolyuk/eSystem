using eShop.Domain.Messages.Email;

namespace eShop.Auth.Api.Interfaces;

public interface IMessageService
{
    public ValueTask SendMessageAsync(string queryName, object message, CancellationToken cancellationToken = default);
}