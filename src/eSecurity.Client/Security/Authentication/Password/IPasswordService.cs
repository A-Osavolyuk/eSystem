using eSecurity.Core.Common.Requests;

namespace eSecurity.Client.Security.Authentication.Password;

public interface IPasswordService
{
    public ValueTask<ApiResponse> AddPasswordAsync(AddPasswordRequest request);
    public ValueTask<ApiResponse> RequestForgotPasswordAsync(ForgotPasswordRequest request);
    public ValueTask<ApiResponse> ConfirmForgotPasswordAsync(ConfirmForgotPasswordRequest request);
    public ValueTask<ApiResponse> ResetPasswordAsync(ResetPasswordRequest request);
    public ValueTask<ApiResponse> SetPasswordAsync(SetPasswordRequest request);
    public ValueTask<ApiResponse> ChangePasswordAsync(ChangePasswordRequest request);
}