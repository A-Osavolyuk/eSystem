using System.Text.Json;
using eSystem.Core.Common.Cache.Redis;
using eSystem.Core.Common.Network.Gateway;
using eSystem.Core.Security.Authentication.Oidc;
using Microsoft.Extensions.Caching.Distributed;

namespace eCinema.Server.Security.Authentication.Oidc;

public class OpenIdDiscoveryProvider(
    IHttpClientFactory httpClientFactory,
    ICacheService cacheService,
    GatewayOptions gatewayOptions) : IOpenIdDiscoveryProvider
{
    private readonly ICacheService _cacheService = cacheService;
    private readonly GatewayOptions _gatewayOptions = gatewayOptions;
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("oidc");
    private const string OpenIdConfigurationKey = "openid_configuration";
    
    public async Task<OpenIdConfiguration> GetOpenIdConfigurationsAsync()
    {
        var openIdConfiguration = await _cacheService.GetAsync<OpenIdConfiguration>(OpenIdConfigurationKey);
        if (openIdConfiguration is not null)
            return openIdConfiguration;

        var requestUri = new Uri($"{_gatewayOptions.Url}/api/v1/connect/.well-known/openid-configuration");
        var response = await _httpClient.GetAsync(requestUri);

        if (!response.IsSuccessStatusCode) 
            throw new Exception();
        
        var responseJson = await response.Content.ReadAsStringAsync();
        openIdConfiguration = JsonSerializer.Deserialize<OpenIdConfiguration>(responseJson)!;
        var options = new DistributedCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromDays(1));
        await _cacheService.SetAsync(OpenIdConfigurationKey, openIdConfiguration, options);
            
        return openIdConfiguration;
    }
}