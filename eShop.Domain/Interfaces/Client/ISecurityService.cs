using eShop.Domain.Common.API;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Domain.Interfaces.Client;

public interface ISecurityService
{
    public ValueTask<Response> LoginAsync(LoginRequest request);
    public ValueTask<Response> RegisterAsync(RegistrationRequest request);
    public ValueTask<Response> RequestResetPasswordAsync(ResetPasswordRequest request);
    public ValueTask<Response> ConfirmResetPasswordAsync(ConfirmResetPasswordRequest request);
    public ValueTask<Response> VerifyEmailAsync(VerifyEmailRequest request);
    public ValueTask<Response> RequestChangeEmailAsync(ChangeEmailRequest request);
    public ValueTask<Response> ConfirmChangeEmailAsync(ConfirmChangeEmailRequest request);
    public ValueTask<Response> ChangePasswordAsync(ChangePasswordRequest request);
    public ValueTask<Response> RefreshTokenAsync(RefreshTokenRequest request);
    public ValueTask<Response> RequestChangePhoneNumberAsync(ChangePhoneNumberRequest request);
    public ValueTask<Response> ConfirmChangePhoneNumberAsync(ConfirmChangePhoneNumberRequest request);
    public ValueTask<Response> ResendVerificationCodeAsync(ResendEmailVerificationCodeRequest request);
    public ValueTask<Response> VerifyCodeAsync(VerifyCodeRequest request);
}