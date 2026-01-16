using eSecurity.Core.Common.Requests;
using eSecurity.Core.Common.Responses;

namespace eSecurity.Client.Security.Authentication.Password;

public interface IPasswordService
{
    public ValueTask<HttpResponse> AddPasswordAsync(AddPasswordRequest request);
    public ValueTask<ApiResponse<ForgotPasswordResponse>> ForgotPasswordAsync(ForgotPasswordRequest request);
    public ValueTask<HttpResponse> ResetPasswordAsync(ResetPasswordRequest request);
    public ValueTask<HttpResponse> ChangePasswordAsync(ChangePasswordRequest request);
}