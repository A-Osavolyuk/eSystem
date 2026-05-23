using eSecurity.Idp.Data.Entities;
using eSystem.Core.Primitives;

namespace eSecurity.Idp.Security.Authentication.EndSession.Backchannel;

public interface IBackchannelLogoutHandler
{
    public ValueTask<Result> HandleAsync(EndSessionRequestEntity request, 
        CancellationToken cancellationToken = default);
}