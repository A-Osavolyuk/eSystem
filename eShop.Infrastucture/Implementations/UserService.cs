using eShop.Domain.Abstraction.Services;
using eShop.Domain.Common.Http;
using eShop.Domain.Enums;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Infrastructure.Implementations;

public class UserService(
    IConfiguration configuration,
    IApiClient apiClient) : ApiService(configuration, apiClient), IUserService
{
    public async ValueTask<HttpResponse> GetUserAsync(Guid id) => await ApiClient.SendAsync(
            new HttpRequest { Url = $"{Gateway}/api/v1/Users/{id}", Method = HttpMethod.Get },
            new HttpOptions { WithBearer = true, Type = DataType.Text });

    public async ValueTask<HttpResponse> GetUserStateAsync(Guid id) => await ApiClient.SendAsync(
            new HttpRequest { Url = $"{Gateway}/api/v1/Users/{id}/state", Method = HttpMethod.Get },
            new HttpOptions { WithBearer = true, Type = DataType.Text });

    public async ValueTask<HttpResponse> GetUserSecurityDataAsync(Guid id) => await ApiClient.SendAsync(
            new HttpRequest { Url = $"{Gateway}/api/v1/Users/{id}/security", Method = HttpMethod.Get },
            new HttpOptions { WithBearer = true, Type = DataType.Text });

    public async ValueTask<HttpResponse> GetUserPersonalDataAsync(Guid id) => await ApiClient.SendAsync(
            new HttpRequest { Url = $"{Gateway}/api/v1/Users/{id}/personal", Method = HttpMethod.Get },
            new HttpOptions { WithBearer = true, Type = DataType.Text });

    public async ValueTask<HttpResponse> GetTwoFactorProvidersAsync(Guid id) => await ApiClient.SendAsync(
            new HttpRequest { Url = $"{Gateway}/api/v1/Users/{id}/2fa/providers", Method = HttpMethod.Get },
            new HttpOptions { WithBearer = false, Type = DataType.Text });

    public async ValueTask<HttpResponse> GetLockoutStateAsync(Guid id) => await ApiClient.SendAsync(
            new HttpRequest { Url = $"{Gateway}/api/v1/Users/{id}/lockout", Method = HttpMethod.Get },
            new HttpOptions { WithBearer = false, Type = DataType.Text });

    public async ValueTask<HttpResponse> ChangeUsernameAsync(ChangeUsernameRequest request) => await ApiClient.SendAsync(
            new HttpRequest { Url = $"{Gateway}/api/v1/Users/{request.UserId}/username", Method = HttpMethod.Patch, Data = request },
            new HttpOptions { WithBearer = true, Type = DataType.Text });

    public async ValueTask<HttpResponse> ChangePersonalDataAsync(ChangePersonalDataRequest request) => await ApiClient.SendAsync(
            new HttpRequest { Url = $"{Gateway}/api/v1/Users/{request.UserId}/personal", Method = HttpMethod.Put, Data = request },
            new HttpOptions { WithBearer = true, Type = DataType.Text });

    public async ValueTask<HttpResponse> RemovePersonalDataAsync(RemovePersonalDataRequest request) => await ApiClient.SendAsync(
            new HttpRequest { Url = $"{Gateway}/api/v1/Users/{request.UserId}/personal", Method = HttpMethod.Delete, Data = request },
            new HttpOptions { WithBearer = true, Type = DataType.Text });

    public async ValueTask<HttpResponse> AddPersonalDataAsync(AddPersonalDataRequest request) => await ApiClient.SendAsync(
            new HttpRequest { Url = $"{Gateway}/api/v1/Users/{request.UserId}/personal", Method = HttpMethod.Post, Data = request },
            new HttpOptions { WithBearer = true, Type = DataType.Text });
}