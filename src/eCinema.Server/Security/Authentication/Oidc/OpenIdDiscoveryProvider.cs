using System.Text.Json;
using eSystem.Core.Common.Cache.Redis;
using eSystem.Core.Common.Gateway;
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
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("OIDC");
    private const string OpenIdConfigurationKey = "openid_configuration";
    
    public async Task<OpenIdConfiguration?> GetOpenIdConfigurationsAsync(CancellationToken cancellationToken = default)
    {
        var configuration = await _cacheService.GetAsync<OpenIdConfiguration>(OpenIdConfigurationKey, cancellationToken);
        if (configuration is not null) return configuration;

        const string requestUri = "/api/v1/connect/.well-known/openid-configuration";
        var response = await _httpClient.GetAsync(requestUri, cancellationToken);
        if (!response.IsSuccessStatusCode) return null;
        
        var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);
        var value = JsonSerializer.Deserialize<OpenIdConfiguration>(responseJson);
        
        var options = new DistributedCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromDays(1));
        await _cacheService.SetAsync(OpenIdConfigurationKey, value, options, cancellationToken);
            
        return value;
    }

    public async Task<JsonWebKeySet?> GetWebKeySetAsync(CancellationToken cancellationToken = default)
    {
        var configuration = await GetOpenIdConfigurationsAsync(cancellationToken);
        if (configuration is null) return null;
        
        var response = await _httpClient.GetAsync(configuration.JwksUri, cancellationToken);
        if (!response.IsSuccessStatusCode) return null;

        var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);
        var serializationOptions = new JsonSerializerOptions()
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        
        return JsonSerializer.Deserialize<JsonWebKeySet>(responseJson, serializationOptions);
    }
}