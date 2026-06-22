using eSecurity.Idp.Data.Entities;
using eSecurity.Idp.Features.Connect;
using eSystem.Core.Primitives;
using eSystem.Core.Server.Security.Authentication.OpenIdConnect.BackchannelAuthentication;

namespace eSecurity.Idp.Security.Authentication.BackchannelAuthentication;

public interface IUserResolver
{
    public Task<TypedResult<UserEntity>> ResolveAsync(BackchannelAuthenticationCommand request,
        CancellationToken cancellationToken = default);
}