using eAccount.Domain.Abstraction.Services;
using eSystem.Core.Common.Network.Gateway;
using eSystem.Core.Requests.Auth;

namespace eAccount.Infrastructure.Implementations;

public class UserService(IApiClient apiClient) : IUserService
{
    private readonly IApiClient apiClient = apiClient;
    private const string BasePath = "api/v1/Users";
    public async ValueTask<HttpResponse> GetUserAsync(Guid id) => await apiClient.SendAsync(
        new HttpRequest { Url = $"{BasePath}/{id}", Method = HttpMethod.Get },
        new HttpOptions { Type = DataType.Text, WithBearer = true });

    public async ValueTask<HttpResponse> GetUserPrimaryEmailAsync(Guid id) => await apiClient.SendAsync(
        new HttpRequest { Url = $"{BasePath}/{id}/primary-email", Method = HttpMethod.Get },
        new HttpOptions { Type = DataType.Text});

    public async ValueTask<HttpResponse> GetUserPrimaryPhoneNumberAsync(Guid id) => await apiClient.SendAsync(
        new HttpRequest { Url = $"{BasePath}/{id}/primary-phone-number", Method = HttpMethod.Get },
        new HttpOptions { Type = DataType.Text });

    public async ValueTask<HttpResponse> GetUserEmailsAsync(Guid id) => await apiClient.SendAsync(
        new HttpRequest { Url = $"{BasePath}/{id}/emails", Method = HttpMethod.Get },
        new HttpOptions { Type = DataType.Text, WithBearer = true });

    public async ValueTask<HttpResponse> GetUserDevicesAsync(Guid id) => await apiClient.SendAsync(
        new HttpRequest { Url = $"{BasePath}/{id}/devices", Method = HttpMethod.Get },
        new HttpOptions { Type = DataType.Text, WithBearer = true });

    public async ValueTask<HttpResponse> GetUserLoginMethodsAsync(Guid id) => await apiClient.SendAsync(
        new HttpRequest { Url = $"{BasePath}/{id}/login-methods", Method = HttpMethod.Get },
        new HttpOptions { Type = DataType.Text, WithBearer = true });

    public async ValueTask<HttpResponse> GetUserVerificationDataAsync(Guid id) => await apiClient.SendAsync(
        new HttpRequest { Url = $"{BasePath}/{id}/verification/data", Method = HttpMethod.Get },
        new HttpOptions { Type = DataType.Text, WithBearer = true });

    public async ValueTask<HttpResponse> GetUserLinkedAccountsDataAsync(Guid id) => await apiClient.SendAsync(
        new HttpRequest { Url = $"{BasePath}/{id}/linked-accounts/data", Method = HttpMethod.Get },
        new HttpOptions { Type = DataType.Text, WithBearer = true });

    public async ValueTask<HttpResponse> GetUserPhoneNumbersAsync(Guid id) => await apiClient.SendAsync(
        new HttpRequest { Url = $"{BasePath}/{id}/phone-numbers", Method = HttpMethod.Get },
        new HttpOptions { Type = DataType.Text, WithBearer = true });

    public async ValueTask<HttpResponse> GetUserStateAsync(Guid id) => await apiClient.SendAsync(
        new HttpRequest { Url = $"{BasePath}/{id}/state", Method = HttpMethod.Get },
        new HttpOptions { Type = DataType.Text, WithBearer = true });

    public async ValueTask<HttpResponse> GetUserPersonalDataAsync(Guid id) => await apiClient.SendAsync(
        new HttpRequest { Url = $"{BasePath}/{id}/personal", Method = HttpMethod.Get },
        new HttpOptions { Type = DataType.Text, WithBearer = true });

    public async ValueTask<HttpResponse> GetTwoFactorMethodsAsync(Guid id) => await apiClient.SendAsync(
        new HttpRequest { Url = $"{BasePath}/{id}/2fa/providers", Method = HttpMethod.Get },
        new HttpOptions { Type = DataType.Text });

    public async ValueTask<HttpResponse> GetLockoutStateAsync(Guid id) => await apiClient.SendAsync(
        new HttpRequest { Url = $"{BasePath}/{id}/lockout", Method = HttpMethod.Get },
        new HttpOptions { Type = DataType.Text });

    public async ValueTask<HttpResponse> ChangeUsernameAsync(ChangeUsernameRequest request) => await apiClient.SendAsync(
        new HttpRequest { Url = $"{BasePath}/{request.UserId}/username", Method = HttpMethod.Patch, Data = request },
        new HttpOptions { Type = DataType.Text, WithBearer = true });

    public async ValueTask<HttpResponse> ChangePersonalDataAsync(ChangePersonalDataRequest request) => await apiClient.SendAsync(
        new HttpRequest { Url = $"{BasePath}/{request.UserId}/personal", Method = HttpMethod.Put, Data = request },
        new HttpOptions { Type = DataType.Text, WithBearer = true });

    public async ValueTask<HttpResponse> RemovePersonalDataAsync(RemovePersonalDataRequest request) => await apiClient.SendAsync(
        new HttpRequest { Url = $"{BasePath}/{request.UserId}/personal", Method = HttpMethod.Delete, Data = request },
        new HttpOptions { Type = DataType.Text, WithBearer = true });

    public async ValueTask<HttpResponse> AddPersonalDataAsync(AddPersonalDataRequest request) => await apiClient.SendAsync(
        new HttpRequest { Url = $"{BasePath}/{request.UserId}/personal", Method = HttpMethod.Post, Data = request },
        new HttpOptions { Type = DataType.Text, WithBearer = true });
}