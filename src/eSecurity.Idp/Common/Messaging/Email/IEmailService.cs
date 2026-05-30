using eSystem.Core.Server.Messaging.Email;

namespace eSecurity.Idp.Common.Messaging.Email;

public interface IEmailService
{
    public ValueTask SendAsync<TContext>(TContext context, CancellationToken cancellationToken = default)
        where TContext : EmailContext;
}