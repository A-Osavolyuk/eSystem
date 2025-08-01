using eShop.Domain.Common.API;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Domain.Interfaces.Client;

public interface ISecurityService
{
    public ValueTask<Response> LoginAsync(LoginRequest request);
    public ValueTask<Response> RegisterAsync(RegistrationRequest request);
    public ValueTask<Response> ForgotPasswordAsync(ForgotPasswordRequest request);
    public ValueTask<Response> ResetPasswordAsync(ResetPasswordRequest request);
    public ValueTask<Response> ResetEmailAsync(ResetEmailRequest request);
    public ValueTask<Response> ConfirmResetEmailAsync(ConfirmResetEmailRequest request);
    public ValueTask<Response> ResetPhoneNumberAsync(ResetPhoneNumberRequest request);
    public ValueTask<Response> ConfirmResetPhoneNumberAsync(ConfirmResetPhoneNumberRequest request);
    public ValueTask<Response> AddPhoneNumberAsync(AddPhoneNumberRequest request);
    public ValueTask<Response> AddRecoveryEmailAsync(AddRecoveryEmailRequest request);
    public ValueTask<Response> ChangeEmailAsync(ChangeEmailRequest request);
    public ValueTask<Response> ConfirmChangeEmailAsync(ConfirmChangeEmailRequest request);
    public ValueTask<Response> ChangePasswordAsync(ChangePasswordRequest request);
    public ValueTask<Response> RefreshTokenAsync(RefreshTokenRequest request);
    public ValueTask<Response> ChangePhoneNumberAsync(ChangePhoneNumberRequest request);
    public ValueTask<Response> ConfirmChangePhoneNumberAsync(ConfirmChangePhoneNumberRequest request);
    public ValueTask<Response> ResendCodeAsync(ResendCodeRequest request);
    public ValueTask<Response> VerifyCodeAsync(VerifyCodeRequest request);
    public ValueTask<Response> VerifyEmailAsync(VerifyEmailRequest request);
    public ValueTask<Response> VerifyCurrentEmailAsync(VerifyCurrentEmailRequest request);
    public ValueTask<Response> VerifyRecoveryEmailAsync(VerifyRecoveryEmailRequest request);
    public ValueTask<Response> VerifyPhoneNumberAsync(VerifyPhoneNumberRequest request);
    public ValueTask<Response> VerifyCurrentPhoneNumberAsync(VerifyCurrentPhoneNumberRequest request);
    public ValueTask<Response> UnlockAccountAsync(UnlockAccountRequest request);
}