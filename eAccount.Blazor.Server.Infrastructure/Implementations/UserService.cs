using eAccount.Blazor.Server.Domain.Abstraction.Services;

namespace eAccount.Blazor.Server.Infrastructure.Implementations;

public class UserService(
    IConfiguration configuration,
    IApiClient apiClient) : ApiService(configuration, apiClient), IUserService
{
    private const string BasePath = "api/v1/Users";
    public async ValueTask<HttpResponse> GetUserAsync(Guid id) => await ApiClient.SendAsync(
        new HttpRequest { Url = $"{Gateway}/{BasePath}/{id}", Method = HttpMethod.Get },
        new HttpOptions { Type = DataType.Text });

    public async ValueTask<HttpResponse> GetUserPrimaryEmailAsync(Guid id) => await ApiClient.SendAsync(
        new HttpRequest { Url = $"{Gateway}/{BasePath}/{id}/primary-email", Method = HttpMethod.Get },
        new HttpOptions { Type = DataType.Text });

    public async ValueTask<HttpResponse> GetUserPrimaryPhoneNumberAsync(Guid id) => await ApiClient.SendAsync(
        new HttpRequest { Url = $"{Gateway}/{BasePath}/{id}/primary-phone-number", Method = HttpMethod.Get },
        new HttpOptions { Type = DataType.Text });

    public async ValueTask<HttpResponse> GetUserEmailsAsync(Guid id) => await ApiClient.SendAsync(
        new HttpRequest { Url = $"{Gateway}/{BasePath}/{id}/emails", Method = HttpMethod.Get },
        new HttpOptions { Type = DataType.Text });

    public async ValueTask<HttpResponse> GetUserDevicesAsync(Guid id) => await ApiClient.SendAsync(
        new HttpRequest { Url = $"{Gateway}/{BasePath}/{id}/devices", Method = HttpMethod.Get },
        new HttpOptions { Type = DataType.Text });

    public async ValueTask<HttpResponse> GetUserLoginMethodsAsync(Guid id) => await ApiClient.SendAsync(
        new HttpRequest { Url = $"{Gateway}/{BasePath}/{id}/login-methods", Method = HttpMethod.Get },
        new HttpOptions { Type = DataType.Text });

    public async ValueTask<HttpResponse> GetUserVerificationDataAsync(Guid id) => await ApiClient.SendAsync(
        new HttpRequest { Url = $"{Gateway}/{BasePath}/{id}/verification/data", Method = HttpMethod.Get },
        new HttpOptions { Type = DataType.Text });

    public async ValueTask<HttpResponse> GetUserLinkedAccountsDataAsync(Guid id) => await ApiClient.SendAsync(
        new HttpRequest { Url = $"{Gateway}/{BasePath}/{id}/linked-accounts/data", Method = HttpMethod.Get },
        new HttpOptions { Type = DataType.Text });

    public async ValueTask<HttpResponse> GetUserPhoneNumbersAsync(Guid id) => await ApiClient.SendAsync(
        new HttpRequest { Url = $"{Gateway}/{BasePath}/{id}/phone-numbers", Method = HttpMethod.Get },
        new HttpOptions { Type = DataType.Text });

    public async ValueTask<HttpResponse> GetUserStateAsync(Guid id) => await ApiClient.SendAsync(
        new HttpRequest { Url = $"{Gateway}/{BasePath}/{id}/state", Method = HttpMethod.Get },
        new HttpOptions { Type = DataType.Text });

    public async ValueTask<HttpResponse> GetUserPersonalDataAsync(Guid id) => await ApiClient.SendAsync(
        new HttpRequest { Url = $"{Gateway}/{BasePath}/{id}/personal", Method = HttpMethod.Get },
        new HttpOptions { Type = DataType.Text });

    public async ValueTask<HttpResponse> GetTwoFactorMethodsAsync(Guid id) => await ApiClient.SendAsync(
        new HttpRequest { Url = $"{Gateway}/{BasePath}/{id}/2fa/providers", Method = HttpMethod.Get },
        new HttpOptions { Type = DataType.Text });

    public async ValueTask<HttpResponse> GetLockoutStateAsync(Guid id) => await ApiClient.SendAsync(
        new HttpRequest { Url = $"{Gateway}/{BasePath}/{id}/lockout", Method = HttpMethod.Get },
        new HttpOptions { Type = DataType.Text });

    public async ValueTask<HttpResponse> ChangeUsernameAsync(ChangeUsernameRequest request) => await ApiClient.SendAsync(
        new HttpRequest { Url = $"{Gateway}/{BasePath}/{request.UserId}/username", Method = HttpMethod.Patch, Data = request },
        new HttpOptions { Type = DataType.Text });

    public async ValueTask<HttpResponse> ChangePersonalDataAsync(ChangePersonalDataRequest request) => await ApiClient.SendAsync(
        new HttpRequest { Url = $"{Gateway}/{BasePath}/{request.UserId}/personal", Method = HttpMethod.Put, Data = request },
        new HttpOptions { Type = DataType.Text });

    public async ValueTask<HttpResponse> RemovePersonalDataAsync(RemovePersonalDataRequest request) => await ApiClient.SendAsync(
        new HttpRequest { Url = $"{Gateway}/{BasePath}/{request.UserId}/personal", Method = HttpMethod.Delete, Data = request },
        new HttpOptions { Type = DataType.Text });

    public async ValueTask<HttpResponse> AddPersonalDataAsync(AddPersonalDataRequest request) => await ApiClient.SendAsync(
        new HttpRequest { Url = $"{Gateway}/{BasePath}/{request.UserId}/personal", Method = HttpMethod.Post, Data = request },
        new HttpOptions { Type = DataType.Text });
}