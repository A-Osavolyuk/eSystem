using eSecurity.Client.Common.Http;
using eSecurity.Core.Common.Requests;
using eSystem.Core.Common.Http;

namespace eSecurity.Client.Security;

public class SecurityService(IApiClient apiClient) : ISecurityService
{
    private readonly IApiClient _apiClient = apiClient;

    public async ValueTask<Result> SignInAsync(SignInRequest request)
        => await _apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/Account/sign-in"
            }, new HttpOptions() { Type = DataType.Text });

    public async ValueTask<Result> SignUpAsync(SignUpRequest request)
        => await _apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/Account/sign-up"
            }, new HttpOptions() { Type = DataType.Text });

    public async ValueTask<Result> AddEmailAsync(AddEmailRequest request)
        => await _apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/Email/add"
            }, new HttpOptions() { Type = DataType.Text, WithBearer = true });

    public async ValueTask<Result> CheckEmailAsync(CheckEmailRequest request)
        => await _apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/Email/check"
            }, new HttpOptions() { Type = DataType.Text });

    public async ValueTask<Result> ChangeEmailAsync(ChangeEmailRequest request)
        => await _apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/Email/change"
            }, new HttpOptions() { Type = DataType.Text, WithBearer = true });

    public async ValueTask<Result> VerifyEmailAsync(VerifyEmailRequest request)
        => await _apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/Email/verify"
            }, new HttpOptions() { Type = DataType.Text });

    public async ValueTask<Result> ManageEmailAsync(ManageEmailRequest request)
        => await _apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/Email/manage"
            }, new HttpOptions() { Type = DataType.Text, WithBearer = true });

    public async ValueTask<Result> RemoveEmailAsync(RemoveEmailRequest request)
        => await _apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/Email/remove"
            }, new HttpOptions() { Type = DataType.Text, WithBearer = true });

    public async ValueTask<Result> ResetEmailAsync(ResetEmailRequest request)
        => await _apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/Email/reset"
            }, new HttpOptions() { Type = DataType.Text });

    public async ValueTask<Result> ChangeUsernameAsync(ChangeUsernameRequest request)
        => await _apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/User/username/change"
            }, new HttpOptions() { Type = DataType.Text, WithBearer = true });

    public async ValueTask<Result> CheckAccountAsync(CheckAccountRequest request)
        => await _apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/Account/check"
            }, new HttpOptions() { Type = DataType.Text });

    public async ValueTask<Result> RecoverAccountAsync(RecoverAccountRequest request)
        => await _apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/Account/recover"
            }, new HttpOptions() { Type = DataType.Text });

    public async ValueTask<Result> UnlockAccountAsync(UnlockAccountRequest request)
        => await _apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/Account/unlock"
            }, new HttpOptions() { Type = DataType.Text });

    public async ValueTask<Result> AddPasswordAsync(AddPasswordRequest request)
        => await _apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/Password/add"
            }, new HttpOptions() { Type = DataType.Text, WithBearer = true });

    public async ValueTask<Result> ForgotPasswordAsync(ForgotPasswordRequest request)
        => await _apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/Password/forgot"
            }, new HttpOptions() { Type = DataType.Text });

    public async ValueTask<Result> ResetPasswordAsync(ResetPasswordRequest request)
        => await _apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/Password/reset"
            }, new HttpOptions() { Type = DataType.Text });

    public async ValueTask<Result> ChangePasswordAsync(ChangePasswordRequest request)
        => await _apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/Password/change"
            }, new HttpOptions() { Type = DataType.Text, WithBearer = true });
}