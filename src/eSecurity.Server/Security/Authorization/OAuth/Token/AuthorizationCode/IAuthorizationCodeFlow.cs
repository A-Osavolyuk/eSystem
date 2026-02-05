using eSecurity.Server.Data.Entities;
using eSystem.Core.Http.Results;

namespace eSecurity.Server.Security.Authorization.OAuth.Token.AuthorizationCode;

public interface IAuthorizationCodeFlow
{
    public ValueTask<Result> ExecuteAsync(AuthorizationCodeContext context, 
        AuthorizationCodeEntity code, CancellationToken cancellationToken = default);
}