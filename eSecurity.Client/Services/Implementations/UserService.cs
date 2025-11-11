using eSecurity.Client.Common.Http;
using eSecurity.Client.Services.Interfaces;
using eSystem.Core.Common.Http;

namespace eSecurity.Client.Services.Implementations;

public class UserService(IApiClient apiClient) : IUserService
{
    private readonly IApiClient apiClient = apiClient;

    public async ValueTask<HttpResponse> GetUserVerificationMethodsAsync(Guid id)
        => await apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Get,
                Url = $"api/v1/User/{id}/verification/methods"
            }, new HttpOptions() { Type = DataType.Text, WithBearer = true });


    public async ValueTask<HttpResponse> GetUserPrimaryEmailAsync(Guid id)
        => await apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Get,
                Url = $"api/v1/User/{id}/emails/primary"
            }, new HttpOptions() { Type = DataType.Text });

    public async ValueTask<HttpResponse> GetUserEmailsAsync(Guid id)
        => await apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Get,
                Url = $"api/v1/User/{id}/emails"
            }, new HttpOptions() { Type = DataType.Text, WithBearer = true });

    public async ValueTask<HttpResponse> GetUserDeviceAsync(Guid id, Guid deviceId)
        => await apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Get,
                Url = $"api/v1/User/{id}/devices/{deviceId}"
            }, new HttpOptions() { Type = DataType.Text });

    public async ValueTask<HttpResponse> GetUserDevicesAsync(Guid id)
        => await apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Get,
                Url = $"api/v1/User/{id}/devices"
            }, new HttpOptions() { Type = DataType.Text, WithBearer = true });

    public async ValueTask<HttpResponse> GetUserLinkedAccountsAsync(Guid id)
        => await apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Get,
                Url = $"api/v1/User/{id}/linked-accounts"
            }, new HttpOptions() { Type = DataType.Text, WithBearer = true });

    public async ValueTask<HttpResponse> GetUserTwoFactorMethodsAsync(Guid id)
        => await apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Get,
                Url = $"api/v1/User/{id}/2fa/methods"
            }, new HttpOptions() { Type = DataType.Text });

    public async ValueTask<HttpResponse> GetUserLoginMethodsAsync(Guid id)
        => await apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Get,
                Url = $"api/v1/User/{id}/login-methods"
            }, new HttpOptions() { Type = DataType.Text, WithBearer = true });
}