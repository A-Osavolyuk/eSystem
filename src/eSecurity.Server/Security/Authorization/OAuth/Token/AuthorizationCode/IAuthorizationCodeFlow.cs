using eSecurity.Server.Data.Entities;

namespace eSecurity.Server.Security.Authorization.OAuth.Token.AuthorizationCode;

public interface IAuthorizationCodeFlow
{
    public ValueTask<Result> ExecuteAsync(AuthorizationCodeEntity code, 
        AuthorizationCodeFlowContext context, CancellationToken cancellationToken = default);
}