using eShop.Domain.Common.API;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Domain.Interfaces.Client;

public interface ISecurityService
{
    public ValueTask<Response> LoginAsync(LoginRequest request);
    public ValueTask<Response> RegisterAsync(RegistrationRequest request);
    public ValueTask<Response> ForgotPasswordAsync(ForgotPasswordRequest request);
    public ValueTask<Response> ResetPasswordAsync(ResetPasswordRequest request);
    public ValueTask<Response> VerifyEmailAsync(VerifyEmailRequest request);
    public ValueTask<Response> VerifyPhoneNumberAsync(VerifyPhoneNumberRequest request);
    public ValueTask<Response> AddPhoneNumberAsync(AddPhoneNumberRequest request);
    public ValueTask<Response> ChangeEmailAsync(ChangeEmailRequest request);
    public ValueTask<Response> ConfirmChangeEmailAsync(ConfirmEmailChangeRequest request);
    public ValueTask<Response> ChangePasswordAsync(ChangePasswordRequest request);
    public ValueTask<Response> RefreshTokenAsync(RefreshTokenRequest request);
    public ValueTask<Response> ChangePhoneNumberAsync(ChangePhoneNumberRequest request);
    public ValueTask<Response> ConfirmChangePhoneNumberAsync(ConfirmChangePhoneNumberRequest request);
    public ValueTask<Response> ResendVerificationCodeAsync(ResendEmailVerificationCodeRequest request);
    public ValueTask<Response> VerifyCodeAsync(VerifyCodeRequest request);
    public ValueTask<Response> RecoverAccountAsync(RecoverAccountRequest request);
}