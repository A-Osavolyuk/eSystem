namespace eShop.Domain.Interfaces.Client;

public interface ISecurityService
{
    public ValueTask<Response> LoginAsync(LoginRequest request);
    public ValueTask<Response> RegisterAsync(RegistrationRequest request);
    public ValueTask<Response> RequestResetPasswordAsync(ResetPasswordRequest request);
    public ValueTask<Response> ConfirmResetPasswordAsync(ConfirmResetPasswordRequest request);
    public ValueTask<Response> VerifyEmailAsync(VerifyEmailRequest request);
    public ValueTask<Response> GetExternalProvidersAsync();
    public ValueTask<Response> LoginWithTwoFactorAuthenticationAsync(LoginWith2FaRequest with2FaRequest);
    public ValueTask<Response> RequestChangeEmailAsync(ChangeEmailRequest request);
    public ValueTask<Response> ConfirmChangeEmailAsync(ConfirmChangeEmailRequest request);
    public ValueTask<Response> ChangePasswordAsync(ChangePasswordRequest request);
    public ValueTask<Response> ChangeTwoFactorAuthenticationStateAsync(Change2FaStateRequest request);
    public ValueTask<Response> GetTwoFactorStateAsync(string email);
    public ValueTask<Response> RefreshToken(RefreshTokenRequest request);
    public ValueTask<Response> RequestChangePhoneNumberAsync(ChangePhoneNumberRequest request);
    public ValueTask<Response> ConfirmChangePhoneNumberAsync(ConfirmChangePhoneNumberRequest request);
    public ValueTask<Response> ResendVerificationCodeAsync(ResendEmailVerificationCodeRequest request);
    public ValueTask<Response> VerifyCodeAsync(VerifyCodeRequest request);
}