using eSecurity.Client.Common.Http;
using eSecurity.Client.Services.Interfaces;
using eSecurity.Core.Common.Requests;
using eSystem.Core.Common.Http;

namespace eSecurity.Client.Services.Implementations;

public class SecurityService(IApiClient apiClient) : ISecurityService
{
    private readonly IApiClient _apiClient = apiClient;

    public async ValueTask<HttpResponse> SignInAsync(SignInRequest request)
        => await _apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/Account/sign-in"
            }, new HttpOptions() { Type = DataType.Text });

    public async ValueTask<HttpResponse> SignUpAsync(SignUpRequest request)
        => await _apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/Account/sign-up"
            }, new HttpOptions() { Type = DataType.Text });

    public async ValueTask<HttpResponse> AddEmailAsync(AddEmailRequest request)
        => await _apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/Email/add"
            }, new HttpOptions() { Type = DataType.Text, WithBearer = true });

    public async ValueTask<HttpResponse> CheckEmailAsync(CheckEmailRequest request)
        => await _apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/Email/check"
            }, new HttpOptions() { Type = DataType.Text });

    public async ValueTask<HttpResponse> ChangeEmailAsync(ChangeEmailRequest request)
        => await _apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/Email/change"
            }, new HttpOptions() { Type = DataType.Text, WithBearer = true });

    public async ValueTask<HttpResponse> VerifyEmailAsync(VerifyEmailRequest request)
        => await _apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/Email/verify"
            }, new HttpOptions() { Type = DataType.Text });

    public async ValueTask<HttpResponse> ManageEmailAsync(ManageEmailRequest request)
        => await _apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/Email/manage"
            }, new HttpOptions() { Type = DataType.Text, WithBearer = true });

    public async ValueTask<HttpResponse> RemoveEmailAsync(RemoveEmailRequest request)
        => await _apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/Email/remove"
            }, new HttpOptions() { Type = DataType.Text, WithBearer = true });

    public async ValueTask<HttpResponse> ResetEmailAsync(ResetEmailRequest request)
        => await _apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/Email/reset"
            }, new HttpOptions() { Type = DataType.Text });

    public async ValueTask<HttpResponse> ChangeUsernameAsync(ChangeUsernameRequest request)
        => await _apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/User/username/change"
            }, new HttpOptions() { Type = DataType.Text, WithBearer = true });

    public async ValueTask<HttpResponse> CheckAccountAsync(CheckAccountRequest request)
        => await _apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/Account/check"
            }, new HttpOptions() { Type = DataType.Text });

    public async ValueTask<HttpResponse> RecoverAccountAsync(RecoverAccountRequest request)
        => await _apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/Account/recover"
            }, new HttpOptions() { Type = DataType.Text });

    public async ValueTask<HttpResponse> UnlockAccountAsync(UnlockAccountRequest request)
        => await _apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/Account/unlock"
            }, new HttpOptions() { Type = DataType.Text });

    public async ValueTask<HttpResponse> AddPasswordAsync(AddPasswordRequest request)
        => await _apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/Password/add"
            }, new HttpOptions() { Type = DataType.Text, WithBearer = true });

    public async ValueTask<HttpResponse> ForgotPasswordAsync(ForgotPasswordRequest request)
        => await _apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/Password/forgot"
            }, new HttpOptions() { Type = DataType.Text });

    public async ValueTask<HttpResponse> ResetPasswordAsync(ResetPasswordRequest request)
        => await _apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/Password/reset"
            }, new HttpOptions() { Type = DataType.Text });

    public async ValueTask<HttpResponse> ChangePasswordAsync(ChangePasswordRequest request)
        => await _apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/Password/change"
            }, new HttpOptions() { Type = DataType.Text, WithBearer = true });
}