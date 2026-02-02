using eSecurity.Server.Data.Entities;
using eSystem.Core.Http.Results;

namespace eSecurity.Server.Security.Authentication.OpenIdConnect.Logout.Backchannel;

public interface IBackchannelLogoutHandler
{
    public ValueTask<Result> ExecuteAsync(SessionEntity session, CancellationToken cancellationToken);
}