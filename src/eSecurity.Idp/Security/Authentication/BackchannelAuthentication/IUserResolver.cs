using eSecurity.Idp.Data.Entities;
using eSystem.Core.Primitives;
using eSystem.Core.Server.Security.Authentication.OpenIdConnect.BackchannelAuthentication;

namespace eSecurity.Idp.Security.Authentication.BackchannelAuthentication;

public interface IUserResolver
{
    public Task<TypedResult<UserEntity>> ResolveAsync(BackchannelAuthenticationRequest request,
        CancellationToken cancellationToken = default);
}