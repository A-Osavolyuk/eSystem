using eSecurity.Server.Data.Entities;
using eSecurity.Server.Security.Authorization.Token.Strategies;
using eSystem.Core.Http.Results;

namespace eSecurity.Server.Security.Authorization.Token.AuthorizationCode;

public interface IAuthorizationCodeFlow
{
    public ValueTask<Result> ExecuteAsync(AuthorizationCodeContext context, 
        AuthorizationCodeEntity code, CancellationToken cancellationToken = default);
}