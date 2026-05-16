using eSystem.Core.Server.Security.Authentication.OpenIdConnect.Discovery;

namespace eSecurity.Client.BFF.Security.Authentication.OpenIdConnect.Discovery;

public interface IOpenIdDiscoveryProvider
{
    ValueTask<OpenIdConfiguration?> GetOpenIdDiscoveryAsync(CancellationToken cancellationToken = default);
    ValueTask<JsonWebKeySet?> GetJsonWebKeySetAsync(CancellationToken cancellationToken);
}