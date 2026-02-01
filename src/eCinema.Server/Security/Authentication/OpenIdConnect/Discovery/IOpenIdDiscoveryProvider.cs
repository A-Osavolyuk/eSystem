using eSystem.Core.Security.Authentication.OpenIdConnect.Discovery;

namespace eCinema.Server.Security.Authentication.OpenIdConnect.Discovery;

public interface IOpenIdDiscoveryProvider
{
    public ValueTask<OpenIdConfiguration?> GetOpenIdDiscoveryAsync(CancellationToken cancellationToken = default);
    public ValueTask<JsonWebKeySet?> GetJsonWebKeySetAsync(CancellationToken cancellationToken = default);
}