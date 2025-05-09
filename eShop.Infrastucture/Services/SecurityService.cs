using eShop.Domain.Abstraction.Services;
using eShop.Domain.Common.API;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Infrastructure.Services;

public class SecurityService(
    IHttpClientService clientService,
    IConfiguration configuration) : ApiService(configuration, clientService), ISecurityService
{
    public async ValueTask<Response> LoginAsync(LoginRequest request) => await HttpClientService.SendAsync(
        new Request(Url: $"{Configuration[Key]}/api/v1/Security/login",
            Method: HttpMethod.Post, Data: request));

    public async ValueTask<Response> ResendVerificationCodeAsync(ResendEmailVerificationCodeRequest request) =>
        await HttpClientService.SendAsync(
            new Request(
                Url: $"{Configuration[Key]}/api/v1/Security/resend-verification-code",
                Method: HttpMethod.Post, Data: request));

    public async ValueTask<Response> VerifyCodeAsync(VerifyCodeRequest request) => await HttpClientService.SendAsync(
        new Request(
            Url: $"{Configuration[Key]}/api/v1/Security/verify-code",
            Method: HttpMethod.Post, Data: request));

    public async ValueTask<Response> RegisterAsync(RegistrationRequest request) => await HttpClientService.SendAsync(
        new Request(Url: $"{Configuration[Key]}/api/v1/Security/register",
            Method: HttpMethod.Post, Data: request));

    public async ValueTask<Response> RequestResetPasswordAsync(ResetPasswordRequest request) =>
        await HttpClientService.SendAsync(
            new Request(
                Url: $"{Configuration[Key]}/api/v1/Security/reset-password",
                Method: HttpMethod.Post, Data: request));

    public async ValueTask<Response> ConfirmResetPasswordAsync(ConfirmResetPasswordRequest request) =>
        await HttpClientService.SendAsync(
            new Request(
                Url: $"{Configuration[Key]}/api/v1/Security/confirm-password",
                Method: HttpMethod.Put, Data: request));

    public async ValueTask<Response> VerifyEmailAsync(VerifyEmailRequest request) => await HttpClientService.SendAsync(
        new Request(Url: $"{Configuration[Key]}/api/v1/Security/verify-email",
            Method: HttpMethod.Post, Data: request));

    public async ValueTask<Response> GetExternalProvidersAsync() => await HttpClientService.SendAsync(
        new Request(Url: $"{Configuration[Key]}/api/v1/Security/get-external-providers",
            Method: HttpMethod.Get));

    public async ValueTask<Response> RequestChangeEmailAsync(ChangeEmailRequest request) =>
        await HttpClientService.SendAsync(
            new Request(
                Url: $"{Configuration[Key]}/api/v1/Security/change-email",
                Method: HttpMethod.Put, Data: request));

    public async ValueTask<Response> ConfirmChangeEmailAsync(ConfirmChangeEmailRequest request) =>
        await HttpClientService.SendAsync(
            new Request(
                Url: $"{Configuration[Key]}/api/v1/Security/confirm-email",
                Method: HttpMethod.Post, Data: request));

    public async ValueTask<Response> ChangePasswordAsync(ChangePasswordRequest request) =>
        await HttpClientService.SendAsync(
            new Request(Url: $"{Configuration[Key]}/api/v1/Security/change-password",
                Method: HttpMethod.Put, Data: request));

    public async ValueTask<Response> RefreshToken(RefreshTokenRequest request) => await HttpClientService.SendAsync(
        new Request(Url: $"{Configuration[Key]}/api/v1/Security/refresh-token",
            Method: HttpMethod.Post, Data: request));

    public async ValueTask<Response> RequestChangePhoneNumberAsync(ChangePhoneNumberRequest request) =>
        await HttpClientService.SendAsync(
            new Request(
                Url: $"{Configuration[Key]}/api/v1/Security/change-phone-number",
                Method: HttpMethod.Put, Data: request));

    public async ValueTask<Response> ConfirmChangePhoneNumberAsync(ConfirmChangePhoneNumberRequest request) =>
        await HttpClientService.SendAsync(
            new Request(
                Url: $"{Configuration[Key]}/api/v1/Security/confirm-phone-number",
                Method: HttpMethod.Post, Data: request));
    
}