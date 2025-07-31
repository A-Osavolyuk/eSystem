using eShop.Domain.Abstraction.Services;
using eShop.Domain.Common.API;
using eShop.Domain.Enums;
using eShop.Domain.Options;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Infrastructure.Services;

public class UsersService(
    IConfiguration configuration, 
    IApiClient apiClient) : ApiService(configuration, apiClient), IUsersService
{
    public async ValueTask<Response> GetUserAsync(Guid id)=>
        await ApiClient.SendAsync(
            new HttpRequest { Url = $"{Gateway}/api/v1/Users/{id}", Method = HttpMethod.Get }, 
            new HttpOptions { WithBearer = true, Type = DataType.Text });
    public async ValueTask<Response> GetTwoFactorProvidersAsync(Guid id) =>
        await ApiClient.SendAsync(
            new HttpRequest { Url = $"{Gateway}/api/v1/Users/{id}/two-factor/providers", Method = HttpMethod.Get }, 
            new HttpOptions { WithBearer = false, Type = DataType.Text });
    
    public async ValueTask<Response> GetLockoutStateAsync(Guid id) =>
        await ApiClient.SendAsync(
            new HttpRequest { Url = $"{Gateway}/api/v1/Users/{id}/lockout-state", Method = HttpMethod.Get }, 
            new HttpOptions { WithBearer = false, Type = DataType.Text });

    public async ValueTask<Response> ChangeUsernameAsync(ChangeUserNameRequest request) =>
        await ApiClient.SendAsync(
            new HttpRequest { Url = $"{Gateway}/api/v1/Users/{request.UserId}/username", Method = HttpMethod.Patch, Data = request }, 
            new HttpOptions { WithBearer = true, Type = DataType.Text });
    
    public async ValueTask<Response> ChangePersonalDataAsync(ChangePersonalDataRequest request) =>
        await ApiClient.SendAsync(
            new HttpRequest { Url = $"{Gateway}/api/v1/Users/{request.UserId}/personal-data", Method = HttpMethod.Put, Data = request }, 
            new HttpOptions { WithBearer = true, Type = DataType.Text });

    public async ValueTask<Response> AddPersonalDataAsync(AddPersonalDataRequest request) =>
        await ApiClient.SendAsync(
            new HttpRequest { Url = $"{Gateway}/api/v1/Users/{request.UserId}/personal-data", Method = HttpMethod.Post, Data = request }, 
            new HttpOptions { WithBearer = true, Type = DataType.Text });
}