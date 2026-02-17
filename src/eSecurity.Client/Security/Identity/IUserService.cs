namespace eSecurity.Client.Security.Identity;

public interface IUserService
{
    public ValueTask<ApiResponse> GetUserVerificationMethodsAsync(string subject);
    public ValueTask<ApiResponse> GetUserEmailsAsync(string subject);
    public ValueTask<ApiResponse> GetUserDevicesAsync(string subject);
    public ValueTask<ApiResponse> GetUserLinkedAccountsAsync(string subject);
    public ValueTask<ApiResponse> GetUserTwoFactorMethodsAsync(string subject);
    public ValueTask<ApiResponse> GetUserLoginMethodsAsync(string subject);
}