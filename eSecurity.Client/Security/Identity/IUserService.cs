namespace eSecurity.Client.Security.Identity;

public interface IUserService
{
    public ValueTask<Result> GetUserVerificationMethodsAsync(Guid id);
    public ValueTask<Result> GetUserPrimaryEmailAsync(Guid id);
    public ValueTask<Result> GetUserEmailsAsync(Guid id);
    public ValueTask<Result> GetUserDeviceAsync(Guid id, Guid deviceId);
    public ValueTask<Result> GetUserDevicesAsync(Guid id);
    public ValueTask<Result> GetUserLinkedAccountsAsync(Guid id);
    public ValueTask<Result> GetUserTwoFactorMethodsAsync(Guid id);
    public ValueTask<Result> GetUserLoginMethodsAsync(Guid id);
}