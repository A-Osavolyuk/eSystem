namespace eShop.Infrastructure.Services;

public class SecurityService(
    IHttpClientService clientService,
    IConfiguration configuration) : ISecurityService
{
    private readonly IHttpClientService clientService = clientService;
    private readonly IConfiguration configuration = configuration;

    public async ValueTask<Response> LoginAsync(LoginRequest request) => await clientService.SendAsync(
        new Request(Url: $"{configuration["Configuration:Services:Proxy:Gateway"]}/api/v1/Auth/login",
            Methods: HttpMethods.Post, Data: request));

    public async ValueTask<Response> ResendVerificationCodeAsync(ResendEmailVerificationCodeRequest request) =>
        await clientService.SendAsync(
            new Request(
                Url: $"{configuration["Configuration:Services:Proxy:Gateway"]}/api/v1/Auth/resend-verification-code",
                Methods: HttpMethods.Post, Data: request));

    public async ValueTask<Response> VerifyCodeAsync(VerifyCodeRequest request) => await clientService.SendAsync(
        new Request(
            Url: $"{configuration["Configuration:Services:Proxy:Gateway"]}/api/v1/Auth/verify-code",
            Methods: HttpMethods.Post, Data: request));

    public async ValueTask<Response> RegisterAsync(RegistrationRequest request) => await clientService.SendAsync(
        new Request(Url: $"{configuration["Configuration:Services:Proxy:Gateway"]}/api/v1/Auth/register",
            Methods: HttpMethods.Post, Data: request));

    public async ValueTask<Response> RequestResetPasswordAsync(ResetPasswordRequest request) =>
        await clientService.SendAsync(
            new Request(
                Url: $"{configuration["Configuration:Services:Proxy:Gateway"]}/api/v1/Auth/request-reset-password",
                Methods: HttpMethods.Post, Data: request));

    public async ValueTask<Response> ConfirmResetPasswordAsync(ConfirmResetPasswordRequest request) =>
        await clientService.SendAsync(
            new Request(
                Url: $"{configuration["Configuration:Services:Proxy:Gateway"]}/api/v1/Auth/confirm-reset-password",
                Methods: HttpMethods.Put, Data: request));

    public async ValueTask<Response> VerifyEmailAsync(VerifyEmailRequest request) => await clientService.SendAsync(
        new Request(Url: $"{configuration["Configuration:Services:Proxy:Gateway"]}/api/v1/Auth/verify-email",
            Methods: HttpMethods.Post, Data: request));

    public async ValueTask<Response> GetExternalProvidersAsync() => await clientService.SendAsync(
        new Request(Url: $"{configuration["Configuration:Services:Proxy:Gateway"]}/api/v1/Auth/get-external-providers",
            Methods: HttpMethods.Get));

    public async ValueTask<Response>
        LoginWithTwoFactorAuthenticationAsync(LoginWith2FaRequest with2FaRequest) =>
        await clientService.SendAsync(new Request(
            Url: $"{configuration["Configuration:Services:Proxy:Gateway"]}/api/v1/Auth/2fa-login",
            Methods: HttpMethods.Post,
            Data: with2FaRequest));

    public async ValueTask<Response> RequestChangeEmailAsync(ChangeEmailRequest request) =>
        await clientService.SendAsync(
            new Request(
                Url: $"{configuration["Configuration:Services:Proxy:Gateway"]}/api/v1/Auth/request-change-email",
                Methods: HttpMethods.Put, Data: request));

    public async ValueTask<Response> ConfirmChangeEmailAsync(ConfirmChangeEmailRequest request) =>
        await clientService.SendAsync(
            new Request(
                Url: $"{configuration["Configuration:Services:Proxy:Gateway"]}/api/v1/Auth/confirm-change-email",
                Methods: HttpMethods.Post, Data: request));

    public async ValueTask<Response> ChangePasswordAsync(ChangePasswordRequest request) =>
        await clientService.SendAsync(
            new Request(Url: $"{configuration["Configuration:Services:Proxy:Gateway"]}/api/v1/Auth/change-password",
                Methods: HttpMethods.Put, Data: request));

    public async ValueTask<Response> ChangeTwoFactorAuthenticationStateAsync(
        Change2FaStateRequest request) => await clientService.SendAsync(
        new Request(Url: $"{configuration["Configuration:Services:Proxy:Gateway"]}/api/v1/Auth/change-2fa-state",
            Methods: HttpMethods.Post, Data: request));

    public async ValueTask<Response> GetTwoFactorStateAsync(string email) => await clientService.SendAsync(
        new Request(Url: $"{configuration["Configuration:Services:Proxy:Gateway"]}/api/v1/Auth/get-2fa-state/{email}",
            Methods: HttpMethods.Get));

    public async ValueTask<Response> RefreshToken(RefreshTokenRequest request) => await clientService.SendAsync(
        new Request(Url: $"{configuration["Configuration:Services:Proxy:Gateway"]}/api/v1/Auth/refresh-token",
            Methods: HttpMethods.Post, Data: request));

    public async ValueTask<Response> RequestChangePhoneNumberAsync(ChangePhoneNumberRequest request) =>
        await clientService.SendAsync(
            new Request(
                Url: $"{configuration["Configuration:Services:Proxy:Gateway"]}/api/v1/Auth/request-change-phone-number",
                Methods: HttpMethods.Put, Data: request));

    public async ValueTask<Response> ConfirmChangePhoneNumberAsync(ConfirmChangePhoneNumberRequest request) =>
        await clientService.SendAsync(
            new Request(
                Url: $"{configuration["Configuration:Services:Proxy:Gateway"]}/api/v1/Auth/confirm-change-phone-number",
                Methods: HttpMethods.Post, Data: request));
    
}