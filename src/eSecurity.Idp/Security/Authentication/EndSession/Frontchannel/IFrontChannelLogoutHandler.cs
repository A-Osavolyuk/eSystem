using eSecurity.Idp.Data.Entities;
using eSystem.Core.Primitives;

namespace eSecurity.Idp.Security.Authentication.EndSession.Frontchannel;

public interface IFrontChannelLogoutHandler
{
    public ValueTask<Result> HandleAsync(EndSessionRequestEntity request,
        CancellationToken cancellationToken = default);
}