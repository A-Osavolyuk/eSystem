namespace eSecurity.Client.Services.Interfaces;

public interface IUserService
{
    public ValueTask<HttpResponse> GetUserVerificationMethodsAsync(Guid id);
    public ValueTask<HttpResponse> GetUserPrimaryEmailAsync(Guid id);
    public ValueTask<HttpResponse> GetUserEmailsAsync(Guid id);
    public ValueTask<HttpResponse> GetUserDeviceAsync(Guid id, Guid deviceId);
    public ValueTask<HttpResponse> GetUserDevicesAsync(Guid id);
    public ValueTask<HttpResponse> GetUserLinkedAccountsAsync(Guid id);
    public ValueTask<HttpResponse> GetUserTwoFactorMethodsAsync(Guid id);
    public ValueTask<HttpResponse> GetUserLoginMethodsAsync(Guid id);
}