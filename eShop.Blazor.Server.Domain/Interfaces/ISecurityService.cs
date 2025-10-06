using eShop.Domain.Common.Http;
using eShop.Domain.Requests.Auth;

namespace eShop.Blazor.Server.Domain.Interfaces;

public interface ISecurityService
{
    public ValueTask<HttpResponse> LoginAsync(LoginRequest request);
    public ValueTask<HttpResponse> RegisterAsync(RegistrationRequest request);
    public ValueTask<HttpResponse> ForgotPasswordAsync(ForgotPasswordRequest request);
    public ValueTask<HttpResponse> ResetPasswordAsync(ResetPasswordRequest request);
    public ValueTask<HttpResponse> ResetEmailAsync(ResetEmailRequest request);
    public ValueTask<HttpResponse> ResetPhoneNumberAsync(ResetPhoneNumberRequest request);
    public ValueTask<HttpResponse> RemovePhoneNumberAsync(RemovePhoneNumberRequest request);
    public ValueTask<HttpResponse> RemoveEmailAsync(RemoveEmailRequest request);
    public ValueTask<HttpResponse> RemovePasswordAsync(RemovePasswordRequest request);
    public ValueTask<HttpResponse> AddPhoneNumberAsync(AddPhoneNumberRequest request);
    public ValueTask<HttpResponse> AddEmailAsync(AddEmailRequest request);
    public ValueTask<HttpResponse> AddPasswordAsync(AddPasswordRequest request);
    public ValueTask<HttpResponse> ChangeEmailAsync(ChangeEmailRequest request);
    public ValueTask<HttpResponse> ManageEmailAsync(ManageEmailRequest request);
    public ValueTask<HttpResponse> ChangePasswordAsync(ChangePasswordRequest request);
    public ValueTask<HttpResponse> ChangePhoneNumberAsync(ChangePhoneNumberRequest request);
    public ValueTask<HttpResponse> VerifyEmailAsync(VerifyEmailRequest request);
    public ValueTask<HttpResponse> VerifyPhoneNumberAsync(VerifyPhoneNumberRequest request);
    public ValueTask<HttpResponse> UnlockAccountAsync(UnlockAccountRequest request);
    public ValueTask<HttpResponse> RecoverAccountAsync(RecoverAccountRequest request);
    public ValueTask<HttpResponse> CheckAccountAsync(CheckAccountRequest request);
    public ValueTask<HttpResponse> CheckEmailAsync(CheckEmailRequest request);
    public ValueTask<HttpResponse> CheckPhoneNumberAsync(CheckPhoneNumberRequest request);
    public ValueTask<HttpResponse> CheckPasswordAsync(CheckPasswordRequest request);
    public ValueTask<HttpResponse> RefreshTokenAsync(RefreshTokenRequest request);
    public ValueTask<HttpResponse> AuthorizeAsync(AuthorizeRequest request);
    public ValueTask<HttpResponse> UnauthorizeAsync(UnauthorizeRequest request);
}