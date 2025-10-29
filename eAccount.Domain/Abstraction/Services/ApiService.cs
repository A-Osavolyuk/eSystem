using eAccount.Domain.Interfaces;
using eSystem.Core.Common.Network.Gateway;
using Microsoft.Extensions.Configuration;

namespace eAccount.Domain.Abstraction.Services;

public abstract class ApiService(GatewayOptions gateway, IApiClient apiClient)
{
    protected GatewayOptions Gateway { get; } = gateway;
    protected IApiClient ApiClient { get; } = apiClient;
}