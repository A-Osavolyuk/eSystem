using eShop.Domain.Interfaces.Client;

namespace eShop.Domain.Abstraction.Services;

using Microsoft.Extensions.Configuration;

public abstract class ApiService(IConfiguration configuration, IHttpClientService httpClientService)
{
    protected readonly IConfiguration Configuration = configuration;
    protected readonly IHttpClientService HttpClientService = httpClientService;
    protected const string Key = "Configuration:Services:Proxy:Gateway:Uri";
}