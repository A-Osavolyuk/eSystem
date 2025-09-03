using eShop.Blazor.Domain.Interfaces;
using Microsoft.Extensions.Configuration;

namespace eShop.Blazor.Domain.Abstraction.Services;

public abstract class ApiService(IConfiguration configuration, IApiClient apiClient)
{
    protected IApiClient ApiClient { get; } = apiClient;
    protected string Gateway { get; } = configuration[Key]!;
    private const string Key = "services:proxy:http:0";
}