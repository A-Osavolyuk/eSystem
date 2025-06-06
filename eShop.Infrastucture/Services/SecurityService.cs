using eShop.Domain.Abstraction.Services;
using eShop.Domain.Common.API;
using eShop.Domain.Options;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Infrastructure.Services;

public class SecurityService(
    IApiClient pipe,
    IConfiguration configuration) : ApiService(configuration, pipe), ISecurityService
{
    public async ValueTask<Response> LoginAsync(LoginRequest request) => await ApiClient.SendAsync(
        new HttpRequest { Url = $"{Gateway}/api/v1/Security/login", Method = HttpMethod.Post, Data = request },
        new HttpOptions { ValidateToken = true, WithBearer = false });
    
    public async ValueTask<Response> ResendVerificationCodeAsync(ResendEmailVerificationCodeRequest request) => await ApiClient.SendAsync(
        new HttpRequest { Url = $"{Gateway}/api/v1/Security/code/resend", Method = HttpMethod.Post, Data = request },
        new HttpOptions { ValidateToken = true, WithBearer = true });
    
    public async ValueTask<Response> VerifyCodeAsync(VerifyCodeRequest request) => await ApiClient.SendAsync(
        new HttpRequest { Url = $"{Gateway}/api/v1/Security/code/verify", Method = HttpMethod.Post, Data = request },
        new HttpOptions { ValidateToken = true, WithBearer = true });
    
    public async ValueTask<Response> RegisterAsync(RegistrationRequest request) => await ApiClient.SendAsync(
        new HttpRequest { Url = $"{Gateway}/api/v1/Security/register", Method = HttpMethod.Post, Data = request },
        new HttpOptions { ValidateToken = true, WithBearer = false });
    
    public async ValueTask<Response> ResetPasswordAsync(ResetPasswordRequest request) => await ApiClient.SendAsync(
        new HttpRequest { Url = $"{Gateway}/api/v1/Security/password/request-reset", Method = HttpMethod.Post, Data = request },
        new HttpOptions { ValidateToken = true, WithBearer = true });
    
    public async ValueTask<Response> ConfirmResetPasswordAsync(ConfirmPasswordResetRequest request) => await ApiClient.SendAsync(
        new HttpRequest { Url = $"{Gateway}/api/v1/Security/password/confirm-reset", Method = HttpMethod.Post, Data = request },
        new HttpOptions { ValidateToken = true, WithBearer = true });
    
    public async ValueTask<Response> VerifyEmailAsync(VerifyEmailRequest request) => await ApiClient.SendAsync(
        new HttpRequest { Url = $"{Gateway}/api/v1/Security/email/verify", Method = HttpMethod.Post, Data = request },
        new HttpOptions { ValidateToken = true, WithBearer = true });
    
    public async ValueTask<Response> ChangeEmailAsync(ChangeEmailRequest request) => await ApiClient.SendAsync(
        new HttpRequest { Url = $"{Gateway}/api/v1/Security/email/request-change", Method = HttpMethod.Post, Data = request },
        new HttpOptions { ValidateToken = true, WithBearer = true });
    
    public async ValueTask<Response> ConfirmChangeEmailAsync(ConfirmEmailChangeRequest request) => await ApiClient.SendAsync(
        new HttpRequest { Url = $"{Gateway}/api/v1/Security/email/confirm-change", Method = HttpMethod.Post, Data = request },
        new HttpOptions { ValidateToken = true, WithBearer = true });
    
    public async ValueTask<Response> ChangePasswordAsync(ChangePasswordRequest request) => await ApiClient.SendAsync(
        new HttpRequest { Url = $"{Gateway}/api/v1/Security/password/change", Method = HttpMethod.Post, Data = request },
        new HttpOptions { ValidateToken = true, WithBearer = true });
    
    public async ValueTask<Response> RefreshTokenAsync(RefreshTokenRequest request) => await ApiClient.SendAsync(
        new HttpRequest { Url = $"{Gateway}/api/v1/Security/refresh-token", Method = HttpMethod.Post, Data = request },
        new HttpOptions { ValidateToken = false, WithBearer = true });
    
    public async ValueTask<Response> ChangePhoneNumberAsync(ChangePhoneNumberRequest request) => await ApiClient.SendAsync(
        new HttpRequest { Url = $"{Gateway}/api/v1/Security/phone-number/request-change", Method = HttpMethod.Post, Data = request },
        new HttpOptions { ValidateToken = true, WithBearer = true });
    
    public async ValueTask<Response> ConfirmChangePhoneNumberAsync(ConfirmChangePhoneNumberRequest request) => await ApiClient.SendAsync(
        new HttpRequest { Url = $"{Gateway}/api/v1/Security/phone-number/confirm-change", Method = HttpMethod.Post, Data = request },
        new HttpOptions { ValidateToken = true, WithBearer = true });

}