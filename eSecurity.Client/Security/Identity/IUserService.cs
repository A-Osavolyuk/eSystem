using eSecurity.Core.Common.DTOs;

namespace eSecurity.Client.Security.Identity;

public interface IUserService
{
    public ValueTask<HttpResponse<UserVerificationData>> GetUserVerificationMethodsAsync(Guid id);
    public ValueTask<HttpResponse<UserEmailDto>> GetUserPrimaryEmailAsync(Guid id);
    public ValueTask<HttpResponse<List<UserEmailDto>>> GetUserEmailsAsync(Guid id);
    public ValueTask<HttpResponse<UserDeviceDto>> GetUserDeviceAsync(Guid id, Guid deviceId);
    public ValueTask<HttpResponse<List<UserDeviceDto>>> GetUserDevicesAsync(Guid id);
    public ValueTask<HttpResponse<UserLinkedAccountData>> GetUserLinkedAccountsAsync(Guid id);
    public ValueTask<HttpResponse<List<UserTwoFactorMethod>>> GetUserTwoFactorMethodsAsync(Guid id);
    public ValueTask<HttpResponse<UserLoginMethodsDto>> GetUserLoginMethodsAsync(Guid id);
}