using eSecurity.Client.Common.Http;
using eSecurity.Core.Requests;

namespace eSecurity.Client.Security.Identity;

public interface IUserService
{
    public ValueTask<ApiResponse> GetUserVerificationMethodsAsync();
    public ValueTask<ApiResponse> GetUserEmailsAsync();
    public ValueTask<ApiResponse> GetUserDevicesAsync();
    public ValueTask<ApiResponse> GetUserLinkedAccountsAsync();
    public ValueTask<ApiResponse> GetUserTwoFactorMethodsAsync();
    public ValueTask<ApiResponse> GetUserLoginMethodsAsync();
    public ValueTask<ApiResponse> SetUsernameAsync(SetUsernameRequest request);
}