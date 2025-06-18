using eShop.Domain.Interfaces.Client;

namespace eShop.Domain.Abstraction.Services;

using Microsoft.Extensions.Configuration;

public abstract class ApiService(IConfiguration configuration, IApiClient apiClient)
{
    protected IApiClient ApiClient { get; } = apiClient;
    protected string Gateway { get; } = configuration[Key]!;
    private const string Key = "services:proxy:http:0";
}