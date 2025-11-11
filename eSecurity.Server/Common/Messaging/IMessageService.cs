using eSystem.Core.Common.Messaging;

namespace eSecurity.Server.Common.Messaging;

public interface IMessageService
{
    public ValueTask SendMessageAsync(SenderType type, Message message, CancellationToken cancellationToken = default);
}