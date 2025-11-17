using eSecurity.Client.Common.Http;
using eSystem.Core.Common.Http;

namespace eSecurity.Client.Security.Identity;

public class UserService(IApiClient apiClient) : IUserService
{
    private readonly IApiClient _apiClient = apiClient;

    public async ValueTask<Result> GetUserVerificationMethodsAsync(Guid id)
        => await _apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Get,
                Url = $"api/v1/User/{id}/verification/methods"
            }, new HttpOptions() { Type = DataType.Text, WithBearer = true });


    public async ValueTask<Result> GetUserPrimaryEmailAsync(Guid id)
        => await _apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Get,
                Url = $"api/v1/User/{id}/emails/primary"
            }, new HttpOptions() { Type = DataType.Text });

    public async ValueTask<Result> GetUserEmailsAsync(Guid id)
        => await _apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Get,
                Url = $"api/v1/User/{id}/emails"
            }, new HttpOptions() { Type = DataType.Text, WithBearer = true });

    public async ValueTask<Result> GetUserDeviceAsync(Guid id, Guid deviceId)
        => await _apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Get,
                Url = $"api/v1/User/{id}/devices/{deviceId}"
            }, new HttpOptions() { Type = DataType.Text });

    public async ValueTask<Result> GetUserDevicesAsync(Guid id)
        => await _apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Get,
                Url = $"api/v1/User/{id}/devices"
            }, new HttpOptions() { Type = DataType.Text, WithBearer = true });

    public async ValueTask<Result> GetUserLinkedAccountsAsync(Guid id)
        => await _apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Get,
                Url = $"api/v1/User/{id}/linked-accounts"
            }, new HttpOptions() { Type = DataType.Text, WithBearer = true });

    public async ValueTask<Result> GetUserTwoFactorMethodsAsync(Guid id)
        => await _apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Get,
                Url = $"api/v1/User/{id}/2fa/methods"
            }, new HttpOptions() { Type = DataType.Text });

    public async ValueTask<Result> GetUserLoginMethodsAsync(Guid id)
        => await _apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Get,
                Url = $"api/v1/User/{id}/login-methods"
            }, new HttpOptions() { Type = DataType.Text, WithBearer = true });
}