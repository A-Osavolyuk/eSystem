using eShop.Domain.Interfaces.Client;

namespace eShop.Domain.Abstraction.Services;

using Microsoft.Extensions.Configuration;

public abstract class ApiService
{
    protected readonly IConfiguration Configuration;
    protected readonly IHttpClientService HttpClientService;
    protected const string Key = "Configuration:Services:Proxy:Gateway:Uri";

    protected ApiService(IConfiguration configuration, IHttpClientService httpClientService)
    {
        this.Configuration = configuration;
        this.HttpClientService = httpClientService;
    }
}