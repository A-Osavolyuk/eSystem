using eSystem.Core.Security.Authentication.Oidc;

namespace eCinema.Server.Security.Authentication.Oidc;

public interface IOpenIdDiscoveryProvider
{
    public Task<OpenIdConfiguration> GetOpenIdConfigurationsAsync();
}