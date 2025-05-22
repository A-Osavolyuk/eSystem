using eShop.Domain.Interfaces.Client;

namespace eShop.Domain.Abstraction.Services;

using Microsoft.Extensions.Configuration;

public abstract class ApiService(IConfiguration configuration, IApiClient apiClient)
{
    protected readonly IConfiguration Configuration = configuration;
    protected readonly IApiClient ApiClient = apiClient;
    protected const string Key = "Configuration:Services:Proxy:Gateway:Uri";
}