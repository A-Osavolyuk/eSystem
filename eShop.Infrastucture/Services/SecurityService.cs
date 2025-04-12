using eShop.Domain.Abstraction.Services;
using eShop.Domain.Common.API;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Infrastructure.Services;

public class SecurityService(
    IHttpClientService clientService,
    IConfiguration configuration) : Api(configuration, clientService), ISecurityService
{
    public async ValueTask<Response> LoginAsync(LoginRequest request) => await HttpClientService.SendAsync(
        new Request(Url: $"{Configuration[Key]}/api/v1/Security/login",
            Methods: HttpMethods.Post, Data: request));

    public async ValueTask<Response> ResendVerificationCodeAsync(ResendEmailVerificationCodeRequest request) =>
        await HttpClientService.SendAsync(
            new Request(
                Url: $"{Configuration[Key]}/api/v1/Security/resend-verification-code",
                Methods: HttpMethods.Post, Data: request));

    public async ValueTask<Response> VerifyCodeAsync(VerifyCodeRequest request) => await HttpClientService.SendAsync(
        new Request(
            Url: $"{Configuration[Key]}/api/v1/Security/verify-code",
            Methods: HttpMethods.Post, Data: request));

    public async ValueTask<Response> RegisterAsync(RegistrationRequest request) => await HttpClientService.SendAsync(
        new Request(Url: $"{Configuration[Key]}/api/v1/Security/register",
            Methods: HttpMethods.Post, Data: request));

    public async ValueTask<Response> RequestResetPasswordAsync(ResetPasswordRequest request) =>
        await HttpClientService.SendAsync(
            new Request(
                Url: $"{Configuration[Key]}/api/v1/Security/request-reset-password",
                Methods: HttpMethods.Post, Data: request));

    public async ValueTask<Response> ConfirmResetPasswordAsync(ConfirmResetPasswordRequest request) =>
        await HttpClientService.SendAsync(
            new Request(
                Url: $"{Configuration[Key]}/api/v1/Security/confirm-reset-password",
                Methods: HttpMethods.Put, Data: request));

    public async ValueTask<Response> VerifyEmailAsync(VerifyEmailRequest request) => await HttpClientService.SendAsync(
        new Request(Url: $"{Configuration[Key]}/api/v1/Security/verify-email",
            Methods: HttpMethods.Post, Data: request));

    public async ValueTask<Response> GetExternalProvidersAsync() => await HttpClientService.SendAsync(
        new Request(Url: $"{Configuration[Key]}/api/v1/Security/get-external-providers",
            Methods: HttpMethods.Get));

    public async ValueTask<Response>
        LoginWithTwoFactorAuthenticationAsync(LoginWith2FaRequest with2FaRequest) =>
        await HttpClientService.SendAsync(new Request(
            Url: $"{Configuration[Key]}/api/v1/Security/2fa-login",
            Methods: HttpMethods.Post,
            Data: with2FaRequest));

    public async ValueTask<Response> RequestChangeEmailAsync(ChangeEmailRequest request) =>
        await HttpClientService.SendAsync(
            new Request(
                Url: $"{Configuration[Key]}/api/v1/Security/request-change-email",
                Methods: HttpMethods.Put, Data: request));

    public async ValueTask<Response> ConfirmChangeEmailAsync(ConfirmChangeEmailRequest request) =>
        await HttpClientService.SendAsync(
            new Request(
                Url: $"{Configuration[Key]}/api/v1/Security/confirm-change-email",
                Methods: HttpMethods.Post, Data: request));

    public async ValueTask<Response> ChangePasswordAsync(ChangePasswordRequest request) =>
        await HttpClientService.SendAsync(
            new Request(Url: $"{Configuration[Key]}/api/v1/Security/change-password",
                Methods: HttpMethods.Put, Data: request));

    public async ValueTask<Response> ChangeTwoFactorAuthenticationStateAsync(
        Change2FaStateRequest request) => await HttpClientService.SendAsync(
        new Request(Url: $"{Configuration[Key]}/api/v1/Security/change-2fa-state",
            Methods: HttpMethods.Post, Data: request));

    public async ValueTask<Response> GetTwoFactorStateAsync(string email) => await HttpClientService.SendAsync(
        new Request(Url: $"{Configuration[Key]}/api/v1/Security/get-2fa-state/{email}",
            Methods: HttpMethods.Get));

    public async ValueTask<Response> RefreshToken(RefreshTokenRequest request) => await HttpClientService.SendAsync(
        new Request(Url: $"{Configuration[Key]}/api/v1/Security/refresh-token",
            Methods: HttpMethods.Post, Data: request));

    public async ValueTask<Response> RequestChangePhoneNumberAsync(ChangePhoneNumberRequest request) =>
        await HttpClientService.SendAsync(
            new Request(
                Url: $"{Configuration[Key]}/api/v1/Security/request-change-phone-number",
                Methods: HttpMethods.Put, Data: request));

    public async ValueTask<Response> ConfirmChangePhoneNumberAsync(ConfirmChangePhoneNumberRequest request) =>
        await HttpClientService.SendAsync(
            new Request(
                Url: $"{Configuration[Key]}/api/v1/Security/confirm-change-phone-number",
                Methods: HttpMethods.Post, Data: request));
    
}