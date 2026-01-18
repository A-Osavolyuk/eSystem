using eSecurity.Core.Common.DTOs;

namespace eSecurity.Client.Security.Identity;

public interface IUserService
{
    public ValueTask<ApiResponse> GetUserVerificationMethodsAsync(Guid id);
    public ValueTask<ApiResponse> GetUserPrimaryEmailAsync(Guid id);
    public ValueTask<ApiResponse> GetUserEmailsAsync(Guid id);
    public ValueTask<ApiResponse> GetUserDeviceAsync(Guid id, Guid deviceId);
    public ValueTask<ApiResponse> GetUserDevicesAsync(Guid id);
    public ValueTask<ApiResponse> GetUserLinkedAccountsAsync(Guid id);
    public ValueTask<ApiResponse> GetUserTwoFactorMethodsAsync(Guid id);
    public ValueTask<ApiResponse> GetUserLoginMethodsAsync(Guid id);
    public ValueTask<ApiResponse> GetUserAsync(Guid id);
}