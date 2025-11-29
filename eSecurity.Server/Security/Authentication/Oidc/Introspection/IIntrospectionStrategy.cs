using eSecurity.Core.Common.Requests;

namespace eSecurity.Server.Security.Authentication.Oidc.Introspection;

public interface IIntrospectionStrategy
{
    public ValueTask<Result> ExecuteAsync(IntrospectionContext context, CancellationToken cancellationToken = default);
}