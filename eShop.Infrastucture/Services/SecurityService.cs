namespace eShop.Infrastructure.Services;

public class SecurityService(
    IHttpClientService clientService,
    IConfiguration configuration) : ISecurityService
{
    private readonly IHttpClientService clientService = clientService;
    private readonly IConfiguration configuration = configuration;

    public async ValueTask<Response> LoginAsync(LoginRequest request) => await clientService.SendAsync(
        new Request(Url: $"{configuration["Configuration:Services:Proxy:Gateway"]}/api/v1/Security/login",
            Methods: HttpMethods.Post, Data: request));

    public async ValueTask<Response> ResendVerificationCodeAsync(ResendEmailVerificationCodeRequest request) =>
        await clientService.SendAsync(
            new Request(
                Url: $"{configuration["Configuration:Services:Proxy:Gateway"]}/api/v1/Security/resend-verification-code",
                Methods: HttpMethods.Post, Data: request));

    public async ValueTask<Response> VerifyCodeAsync(VerifyCodeRequest request) => await clientService.SendAsync(
        new Request(
            Url: $"{configuration["Configuration:Services:Proxy:Gateway"]}/api/v1/Security/verify-code",
            Methods: HttpMethods.Post, Data: request));

    public async ValueTask<Response> RegisterAsync(RegistrationRequest request) => await clientService.SendAsync(
        new Request(Url: $"{configuration["Configuration:Services:Proxy:Gateway"]}/api/v1/Security/register",
            Methods: HttpMethods.Post, Data: request));

    public async ValueTask<Response> RequestResetPasswordAsync(ResetPasswordRequest request) =>
        await clientService.SendAsync(
            new Request(
                Url: $"{configuration["Configuration:Services:Proxy:Gateway"]}/api/v1/Security/request-reset-password",
                Methods: HttpMethods.Post, Data: request));

    public async ValueTask<Response> ConfirmResetPasswordAsync(ConfirmResetPasswordRequest request) =>
        await clientService.SendAsync(
            new Request(
                Url: $"{configuration["Configuration:Services:Proxy:Gateway"]}/api/v1/Security/confirm-reset-password",
                Methods: HttpMethods.Put, Data: request));

    public async ValueTask<Response> VerifyEmailAsync(VerifyEmailRequest request) => await clientService.SendAsync(
        new Request(Url: $"{configuration["Configuration:Services:Proxy:Gateway"]}/api/v1/Security/verify-email",
            Methods: HttpMethods.Post, Data: request));

    public async ValueTask<Response> GetExternalProvidersAsync() => await clientService.SendAsync(
        new Request(Url: $"{configuration["Configuration:Services:Proxy:Gateway"]}/api/v1/Security/get-external-providers",
            Methods: HttpMethods.Get));

    public async ValueTask<Response>
        LoginWithTwoFactorAuthenticationAsync(LoginWith2FaRequest with2FaRequest) =>
        await clientService.SendAsync(new Request(
            Url: $"{configuration["Configuration:Services:Proxy:Gateway"]}/api/v1/Security/2fa-login",
            Methods: HttpMethods.Post,
            Data: with2FaRequest));

    public async ValueTask<Response> RequestChangeEmailAsync(ChangeEmailRequest request) =>
        await clientService.SendAsync(
            new Request(
                Url: $"{configuration["Configuration:Services:Proxy:Gateway"]}/api/v1/Security/request-change-email",
                Methods: HttpMethods.Put, Data: request));

    public async ValueTask<Response> ConfirmChangeEmailAsync(ConfirmChangeEmailRequest request) =>
        await clientService.SendAsync(
            new Request(
                Url: $"{configuration["Configuration:Services:Proxy:Gateway"]}/api/v1/Security/confirm-change-email",
                Methods: HttpMethods.Post, Data: request));

    public async ValueTask<Response> ChangePasswordAsync(ChangePasswordRequest request) =>
        await clientService.SendAsync(
            new Request(Url: $"{configuration["Configuration:Services:Proxy:Gateway"]}/api/v1/Security/change-password",
                Methods: HttpMethods.Put, Data: request));

    public async ValueTask<Response> ChangeTwoFactorAuthenticationStateAsync(
        Change2FaStateRequest request) => await clientService.SendAsync(
        new Request(Url: $"{configuration["Configuration:Services:Proxy:Gateway"]}/api/v1/Security/change-2fa-state",
            Methods: HttpMethods.Post, Data: request));

    public async ValueTask<Response> GetTwoFactorStateAsync(string email) => await clientService.SendAsync(
        new Request(Url: $"{configuration["Configuration:Services:Proxy:Gateway"]}/api/v1/Security/get-2fa-state/{email}",
            Methods: HttpMethods.Get));

    public async ValueTask<Response> RefreshToken(RefreshTokenRequest request) => await clientService.SendAsync(
        new Request(Url: $"{configuration["Configuration:Services:Proxy:Gateway"]}/api/v1/Security/refresh-token",
            Methods: HttpMethods.Post, Data: request));

    public async ValueTask<Response> RequestChangePhoneNumberAsync(ChangePhoneNumberRequest request) =>
        await clientService.SendAsync(
            new Request(
                Url: $"{configuration["Configuration:Services:Proxy:Gateway"]}/api/v1/Security/request-change-phone-number",
                Methods: HttpMethods.Put, Data: request));

    public async ValueTask<Response> ConfirmChangePhoneNumberAsync(ConfirmChangePhoneNumberRequest request) =>
        await clientService.SendAsync(
            new Request(
                Url: $"{configuration["Configuration:Services:Proxy:Gateway"]}/api/v1/Security/confirm-change-phone-number",
                Methods: HttpMethods.Post, Data: request));
    
}