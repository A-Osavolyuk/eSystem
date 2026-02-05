using eSystem.Core.Http.Results;
using eSystem.Core.Security.Authorization.OAuth.Token;

namespace eSecurity.Server.Security.Authorization.OAuth.Token;

public interface ITokenStrategy
{
    public ValueTask<Result> ExecuteAsync(TokenRequest tokenRequest, CancellationToken cancellationToken = default);
}