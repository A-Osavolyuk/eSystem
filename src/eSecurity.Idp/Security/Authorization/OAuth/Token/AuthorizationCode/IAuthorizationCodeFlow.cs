using eSecurity.Idp.Data.Entities;
using eSystem.Core.Primitives;

namespace eSecurity.Idp.Security.Authorization.OAuth.Token.AuthorizationCode;

public interface IAuthorizationCodeFlow
{
    public ValueTask<Result> ExecuteAsync(AuthorizationCodeEntity code, 
        AuthorizationCodeFlowContext context, CancellationToken cancellationToken = default);
}