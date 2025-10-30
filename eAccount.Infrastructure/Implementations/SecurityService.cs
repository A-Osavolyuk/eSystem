using eAccount.Domain.Abstraction.Services;
using eSystem.Core.Common.Network.Gateway;
using eSystem.Core.Requests.Auth;

namespace eAccount.Infrastructure.Implementations;

public class SecurityService(IApiClient apiClient) : ISecurityService
{
    private readonly IApiClient apiClient = apiClient;
    private const string BasePath = "api/v1/Security";

    public async ValueTask<HttpResponse> SignInAsync(SignInRequest request) => await apiClient.SendAsync(
        new HttpRequest { Url = $"{BasePath}/sign-in", Method = HttpMethod.Post, Data = request },
        new HttpOptions { Type = DataType.Text });

    public async ValueTask<HttpResponse> SignUpAsync(SignUpRequest request) => await apiClient.SendAsync(
        new HttpRequest { Url = $"{BasePath}/sign-up", Method = HttpMethod.Post, Data = request },
        new HttpOptions { Type = DataType.Text });
    
    public async ValueTask<HttpResponse> ForgotPasswordAsync(ForgotPasswordRequest request) => await apiClient.SendAsync(
        new HttpRequest { Url = $"{BasePath}/password/forgot", Method = HttpMethod.Post, Data = request },
        new HttpOptions { Type = DataType.Text });
    
    public async ValueTask<HttpResponse> ResetPasswordAsync(ResetPasswordRequest request) => await apiClient.SendAsync(
        new HttpRequest { Url = $"{BasePath}/password/reset", Method = HttpMethod.Post, Data = request },
        new HttpOptions { Type = DataType.Text });

    public async ValueTask<HttpResponse> ResetEmailAsync(ResetEmailRequest request) => await apiClient.SendAsync(
        new HttpRequest { Url = $"{BasePath}/email/reset", Method = HttpMethod.Post, Data = request },
        new HttpOptions { Type = DataType.Text, WithBearer = true });

    public async ValueTask<HttpResponse> VerifyEmailAsync(VerifyEmailRequest request) => await apiClient.SendAsync(
        new HttpRequest { Url = $"{BasePath}/email/verify", Method = HttpMethod.Post, Data = request },
        new HttpOptions { Type = DataType.Text });

    public async ValueTask<HttpResponse> AddEmailAsync(AddEmailRequest request) => await apiClient.SendAsync(
        new HttpRequest { Url = $"{BasePath}/email/add", Method = HttpMethod.Post, Data = request },
        new HttpOptions { Type = DataType.Text, WithBearer = true });

    public async ValueTask<HttpResponse> AddPasswordAsync(AddPasswordRequest request) => await apiClient.SendAsync(
        new HttpRequest { Url = $"{BasePath}/password/add", Method = HttpMethod.Post, Data = request },
        new HttpOptions { Type = DataType.Text, WithBearer = true });

    public async ValueTask<HttpResponse> ChangeEmailAsync(ChangeEmailRequest request) => await apiClient.SendAsync(
        new HttpRequest { Url = $"{BasePath}/email/change", Method = HttpMethod.Post, Data = request },
        new HttpOptions { Type = DataType.Text, WithBearer = true });

    public async ValueTask<HttpResponse> ManageEmailAsync(ManageEmailRequest request) => await apiClient.SendAsync(
        new HttpRequest { Url = $"{BasePath}/email/manage", Method = HttpMethod.Post, Data = request },
        new HttpOptions { Type = DataType.Text, WithBearer = true });

    public async ValueTask<HttpResponse> ChangePasswordAsync(ChangePasswordRequest request) => await apiClient.SendAsync(
        new HttpRequest { Url = $"{BasePath}/password/change", Method = HttpMethod.Post, Data = request },
        new HttpOptions { Type = DataType.Text, WithBearer = true });
    public async ValueTask<HttpResponse> UnlockAccountAsync(UnlockAccountRequest request) => await apiClient.SendAsync(
        new HttpRequest { Url = $"{BasePath}/account/unlock", Method = HttpMethod.Post, Data = request },
        new HttpOptions { Type = DataType.Text });

    public async ValueTask<HttpResponse> RecoverAccountAsync(RecoverAccountRequest request) => await apiClient.SendAsync(
        new HttpRequest { Url = $"{BasePath}/account/recover", Method = HttpMethod.Post, Data = request },
        new HttpOptions { Type = DataType.Text });

    public async ValueTask<HttpResponse> CheckAccountAsync(CheckAccountRequest request) => await apiClient.SendAsync(
        new HttpRequest { Url = $"{BasePath}/account/check", Method = HttpMethod.Post, Data = request },
        new HttpOptions { Type = DataType.Text });

    public async ValueTask<HttpResponse> CheckEmailAsync(CheckEmailRequest request) => await apiClient.SendAsync(
        new HttpRequest { Url = $"{BasePath}/email/check", Method = HttpMethod.Post, Data = request },
        new HttpOptions { Type = DataType.Text });

    public async ValueTask<HttpResponse> CheckPhoneNumberAsync(CheckPhoneNumberRequest request) => await apiClient.SendAsync(
        new HttpRequest { Url = $"{BasePath}/phone-number/check", Method = HttpMethod.Post, Data = request },
        new HttpOptions { Type = DataType.Text });

    public async ValueTask<HttpResponse> CheckPasswordAsync(CheckPasswordRequest request) => await apiClient.SendAsync(
        new HttpRequest { Url = $"{BasePath}/password/check", Method = HttpMethod.Post, Data = request },
        new HttpOptions { Type = DataType.Text });

    public async ValueTask<HttpResponse> ChangePhoneNumberAsync(ChangePhoneNumberRequest request) => await apiClient.SendAsync(
        new HttpRequest { Url = $"{BasePath}/phone-number/change", Method = HttpMethod.Post, Data = request },
        new HttpOptions { Type = DataType.Text, WithBearer = true });
    
    public async ValueTask<HttpResponse> VerifyPhoneNumberAsync(VerifyPhoneNumberRequest request) => await apiClient.SendAsync(
        new HttpRequest { Url = $"{BasePath}/phone-number/verify", Method = HttpMethod.Post, Data = request },
        new HttpOptions { Type = DataType.Text });

    public async ValueTask<HttpResponse> RemoveEmailAsync(RemoveEmailRequest request) => await apiClient.SendAsync(
        new HttpRequest { Url = $"{BasePath}/email/remove", Method = HttpMethod.Post, Data = request },
        new HttpOptions { Type = DataType.Text, WithBearer = true });

    public async ValueTask<HttpResponse> RemovePasswordAsync(RemovePasswordRequest request) => await apiClient.SendAsync(
        new HttpRequest { Url = $"{BasePath}/password/remove", Method = HttpMethod.Post, Data = request },
        new HttpOptions { Type = DataType.Text, WithBearer = true });

    public async ValueTask<HttpResponse> AddPhoneNumberAsync(AddPhoneNumberRequest request) => await apiClient.SendAsync(
        new HttpRequest { Url = $"{BasePath}/phone-number/add", Method = HttpMethod.Post, Data = request },
        new HttpOptions { Type = DataType.Text, WithBearer = true });
    
    public async ValueTask<HttpResponse> ResetPhoneNumberAsync(ResetPhoneNumberRequest request) => await apiClient.SendAsync(
        new HttpRequest { Url = $"{BasePath}/phone-number/reset", Method = HttpMethod.Post, Data = request },
        new HttpOptions { Type = DataType.Text, WithBearer = true });

    public async ValueTask<HttpResponse> RemovePhoneNumberAsync(RemovePhoneNumberRequest request) => await apiClient.SendAsync(
        new HttpRequest { Url = $"{BasePath}/phone-number/remove", Method = HttpMethod.Post, Data = request },
        new HttpOptions { Type = DataType.Text, WithBearer = true });
    
}