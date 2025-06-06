using eShop.Domain.Abstraction.Services;
using eShop.Domain.Common.API;
using eShop.Domain.Enums;
using eShop.Domain.Options;

namespace eShop.Infrastructure.Services;

public class UsersService(
    IConfiguration configuration, 
    IApiClient apiClient) : ApiService(configuration, apiClient), IUsersService
{
    public async ValueTask<Response> GetTwoFactorProvidersAsync(Guid id) =>
        await ApiClient.SendAsync(
            new HttpRequest { Url = $"{Gateway}/api/v1/Users/{id}/two-factor-providers", Method = HttpMethod.Get }, 
            new HttpOptions { ValidateToken = false, WithBearer = false, Type = DataType.Text });
    
    public async ValueTask<Response> GetTwoFactorStateAsync(Guid id) =>
        await ApiClient.SendAsync(
            new HttpRequest { Url = $"{Gateway}/api/v1/Users/{id}/two-factor-state", Method = HttpMethod.Get }, 
            new HttpOptions { ValidateToken = true, WithBearer = true, Type = DataType.Text });
    
    public async ValueTask<Response> GetLockoutStateAsync(Guid id) =>
        await ApiClient.SendAsync(
            new HttpRequest { Url = $"{Gateway}/api/v1/Users/{id}/lockout-state", Method = HttpMethod.Get }, 
            new HttpOptions { ValidateToken = false, WithBearer = false, Type = DataType.Text });

    public async ValueTask<Response> GetPersonalDataAsync(Guid id) =>
        await ApiClient.SendAsync(
            new HttpRequest { Url = $"{Gateway}/api/v1/Users/{id}/personal-data", Method = HttpMethod.Get }, 
            new HttpOptions { ValidateToken = true, WithBearer = true, Type = DataType.Text });
}