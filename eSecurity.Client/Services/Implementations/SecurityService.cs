using eSecurity.Client.Common.Http;
using eSecurity.Client.Services.Interfaces;
using eSecurity.Core.Common.Requests;
using eSystem.Core.Common.Http;

namespace eSecurity.Client.Services.Implementations;

public class SecurityService(IApiClient apiClient) : ISecurityService
{
    private readonly IApiClient apiClient = apiClient;

    public async ValueTask<HttpResponse> SignInAsync(SignInRequest request)
        => await apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/Account/sign-in"
            }, new HttpOptions() { Type = DataType.Text });

    public async ValueTask<HttpResponse> SignUpAsync(SignUpRequest request)
        => await apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/Account/sign-up"
            }, new HttpOptions() { Type = DataType.Text });

    public async ValueTask<HttpResponse> AddEmailAsync(AddEmailRequest request)
        => await apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/Email/add"
            }, new HttpOptions() { Type = DataType.Text, WithBearer = true });

    public async ValueTask<HttpResponse> CheckEmailAsync(CheckEmailRequest request)
        => await apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/Email/check"
            }, new HttpOptions() { Type = DataType.Text });

    public async ValueTask<HttpResponse> ChangeEmailAsync(ChangeEmailRequest request)
        => await apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/Email/change"
            }, new HttpOptions() { Type = DataType.Text, WithBearer = true });

    public async ValueTask<HttpResponse> VerifyEmailAsync(VerifyEmailRequest request)
        => await apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/Email/verify"
            }, new HttpOptions() { Type = DataType.Text });

    public async ValueTask<HttpResponse> ManageEmailAsync(ManageEmailRequest request)
        => await apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/Email/manage"
            }, new HttpOptions() { Type = DataType.Text, WithBearer = true });

    public async ValueTask<HttpResponse> RemoveEmailAsync(RemoveEmailRequest request)
        => await apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/Email/remove"
            }, new HttpOptions() { Type = DataType.Text, WithBearer = true });

    public async ValueTask<HttpResponse> ResetEmailAsync(ResetEmailRequest request)
        => await apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/Email/reset"
            }, new HttpOptions() { Type = DataType.Text });

    public async ValueTask<HttpResponse> ChangeUsernameAsync(ChangeUsernameRequest request)
        => await apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/User/username/change"
            }, new HttpOptions() { Type = DataType.Text, WithBearer = true });

    public async ValueTask<HttpResponse> CheckAccountAsync(CheckAccountRequest request)
        => await apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/Account/check"
            }, new HttpOptions() { Type = DataType.Text });

    public async ValueTask<HttpResponse> RecoverAccountAsync(RecoverAccountRequest request)
        => await apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/Account/recover"
            }, new HttpOptions() { Type = DataType.Text });

    public async ValueTask<HttpResponse> UnlockAccountAsync(UnlockAccountRequest request)
        => await apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/Account/unlock"
            }, new HttpOptions() { Type = DataType.Text });

    public async ValueTask<HttpResponse> AddPasswordAsync(AddPasswordRequest request)
        => await apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/Password/add"
            }, new HttpOptions() { Type = DataType.Text, WithBearer = true });

    public async ValueTask<HttpResponse> ForgotPasswordAsync(ForgotPasswordRequest request)
        => await apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/Password/forgot"
            }, new HttpOptions() { Type = DataType.Text });

    public async ValueTask<HttpResponse> ResetPasswordAsync(ResetPasswordRequest request)
        => await apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/Password/reset"
            }, new HttpOptions() { Type = DataType.Text });

    public async ValueTask<HttpResponse> ChangePasswordAsync(ChangePasswordRequest request)
        => await apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/Password/change"
            }, new HttpOptions() { Type = DataType.Text, WithBearer = true });
}