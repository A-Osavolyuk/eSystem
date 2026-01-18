using eSecurity.Core.Common.Requests;

namespace eSecurity.Client.Security.Identity;

public interface IEmailService
{
    public ValueTask<ApiResponse> AddEmailAsync(AddEmailRequest request);
    public ValueTask<ApiResponse> CheckEmailAsync(CheckEmailRequest request);
    public ValueTask<ApiResponse> ChangeEmailAsync(ChangeEmailRequest request);
    public ValueTask<ApiResponse> VerifyEmailAsync(VerifyEmailRequest request);
    public ValueTask<ApiResponse> ManageEmailAsync(ManageEmailRequest request);
    public ValueTask<ApiResponse> RemoveEmailAsync(RemoveEmailRequest request);
    public ValueTask<ApiResponse> ResetEmailAsync(ResetEmailRequest request);
}