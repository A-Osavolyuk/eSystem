using eAccount.Domain.Interfaces;
using Microsoft.Extensions.Configuration;

namespace eAccount.Domain.Abstraction.Services;

public abstract class ApiService(IConfiguration configuration, IApiClient apiClient)
{
    protected IApiClient ApiClient { get; } = apiClient;
    protected string Gateway { get; } = configuration[Key]!;
    private const string Key = "services:proxy:http:0";
}