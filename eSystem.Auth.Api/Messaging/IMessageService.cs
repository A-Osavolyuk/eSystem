using eSystem.Core.Common.Messaging;

namespace eSystem.Auth.Api.Messaging;

public interface IMessageService
{
    public ValueTask SendMessageAsync(SenderType type, Message message, CancellationToken cancellationToken = default);
}