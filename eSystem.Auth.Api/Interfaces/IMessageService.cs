using eSystem.Core.Common.Messaging;

namespace eSystem.Auth.Api.Interfaces;

public interface IMessageService
{
    public ValueTask SendMessageAsync(SenderType type, Message message, CancellationToken cancellationToken = default);
}