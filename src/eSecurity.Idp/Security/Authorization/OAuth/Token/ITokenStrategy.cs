using eSystem.Core.Primitives;
using eSystem.Core.Server.Security.Authorization.OAuth.Token;

namespace eSecurity.Idp.Security.Authorization.OAuth.Token;

public interface ITokenStrategy
{
    public ValueTask<Result> ExecuteAsync(TokenRequest tokenRequest, CancellationToken cancellationToken = default);
}