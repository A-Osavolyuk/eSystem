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
        new HttpOptions { ValidateToken = true, WithBearer = true });
    
    public async ValueTask<Response> ResendVerificationCodeAsync(ResendEmailVerificationCodeRequest request) => await ApiClient.SendAsync(
        new HttpRequest { Url = $"{Gateway}/api/v1/Security/resend-verification-code", Method = HttpMethod.Post, Data = request },
        new HttpOptions { ValidateToken = true, WithBearer = true });
    
    public async ValueTask<Response> VerifyCodeAsync(VerifyCodeRequest request) => await ApiClient.SendAsync(
        new HttpRequest { Url = $"{Gateway}/api/v1/Security/verify-code", Method = HttpMethod.Post, Data = request },
        new HttpOptions { ValidateToken = true, WithBearer = true });
    
    public async ValueTask<Response> RegisterAsync(RegistrationRequest request) => await ApiClient.SendAsync(
        new HttpRequest { Url = $"{Gateway}/api/v1/Security/register", Method = HttpMethod.Post, Data = request },
        new HttpOptions { ValidateToken = true, WithBearer = true });
    
    public async ValueTask<Response> RequestResetPasswordAsync(ResetPasswordRequest request) => await ApiClient.SendAsync(
        new HttpRequest { Url = $"{Gateway}/api/v1/Security/reset-password", Method = HttpMethod.Post, Data = request },
        new HttpOptions { ValidateToken = true, WithBearer = true });
    
    public async ValueTask<Response> ConfirmResetPasswordAsync(ConfirmResetPasswordRequest request) => await ApiClient.SendAsync(
        new HttpRequest { Url = $"{Gateway}/api/v1/Security/confirm-password", Method = HttpMethod.Post, Data = request },
        new HttpOptions { ValidateToken = true, WithBearer = true });
    
    public async ValueTask<Response> VerifyEmailAsync(VerifyEmailRequest request) => await ApiClient.SendAsync(
        new HttpRequest { Url = $"{Gateway}/api/v1/Security/verify-email", Method = HttpMethod.Post, Data = request },
        new HttpOptions { ValidateToken = true, WithBearer = true });
    
    public async ValueTask<Response> RequestChangeEmailAsync(ChangeEmailRequest request) => await ApiClient.SendAsync(
        new HttpRequest { Url = $"{Gateway}/api/v1/Security/change-email", Method = HttpMethod.Post, Data = request },
        new HttpOptions { ValidateToken = true, WithBearer = true });
    
    public async ValueTask<Response> ConfirmChangeEmailAsync(ConfirmChangeEmailRequest request) => await ApiClient.SendAsync(
        new HttpRequest { Url = $"{Gateway}/api/v1/Security/confirm-email", Method = HttpMethod.Post, Data = request },
        new HttpOptions { ValidateToken = true, WithBearer = true });
    
    public async ValueTask<Response> ChangePasswordAsync(ChangePasswordRequest request) => await ApiClient.SendAsync(
        new HttpRequest { Url = $"{Gateway}/api/v1/Security/change-password", Method = HttpMethod.Post, Data = request },
        new HttpOptions { ValidateToken = true, WithBearer = true });
    
    public async ValueTask<Response> RefreshTokenAsync(RefreshTokenRequest request) => await ApiClient.SendAsync(
        new HttpRequest { Url = $"{Gateway}/api/v1/Security/refresh-token", Method = HttpMethod.Post, Data = request },
        new HttpOptions { ValidateToken = false, WithBearer = true });
    
    public async ValueTask<Response> RequestChangePhoneNumberAsync(ChangePhoneNumberRequest request) => await ApiClient.SendAsync(
        new HttpRequest { Url = $"{Gateway}/api/v1/Security/change-phone-number", Method = HttpMethod.Post, Data = request },
        new HttpOptions { ValidateToken = true, WithBearer = true });
    
    public async ValueTask<Response> ConfirmChangePhoneNumberAsync(ConfirmChangePhoneNumberRequest request) => await ApiClient.SendAsync(
        new HttpRequest { Url = $"{Gateway}/api/v1/Security/confirm-phone-number", Method = HttpMethod.Post, Data = request },
        new HttpOptions { ValidateToken = true, WithBearer = true });

}