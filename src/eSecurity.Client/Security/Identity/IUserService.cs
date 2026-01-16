using eSecurity.Core.Common.DTOs;

namespace eSecurity.Client.Security.Identity;

public interface IUserService
{
    public ValueTask<ApiResponse<UserVerificationData>> GetUserVerificationMethodsAsync(Guid id);
    public ValueTask<ApiResponse<UserEmailDto>> GetUserPrimaryEmailAsync(Guid id);
    public ValueTask<ApiResponse<List<UserEmailDto>>> GetUserEmailsAsync(Guid id);
    public ValueTask<ApiResponse<UserDeviceDto>> GetUserDeviceAsync(Guid id, Guid deviceId);
    public ValueTask<ApiResponse<List<UserDeviceDto>>> GetUserDevicesAsync(Guid id);
    public ValueTask<ApiResponse<UserLinkedAccountData>> GetUserLinkedAccountsAsync(Guid id);
    public ValueTask<ApiResponse<List<UserTwoFactorMethod>>> GetUserTwoFactorMethodsAsync(Guid id);
    public ValueTask<ApiResponse<UserLoginMethodsDto>> GetUserLoginMethodsAsync(Guid id);
    public ValueTask<ApiResponse<UserDto>> GetUserAsync(Guid id);
}