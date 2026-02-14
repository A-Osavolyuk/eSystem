using eSecurity.Server.Data.Entities;
using eSystem.Core.Security.Authentication.OpenIdConnect.BackchannelAuthentication;

namespace eSecurity.Server.Security.Authentication.OpenIdConnect.BackchannelAuthentication;

public interface IUserResolver
{
    public Task<UserResolveResult> ResolveAsync(BackchannelAuthenticationRequest request,
        CancellationToken cancellationToken = default);
}