using eShop.Domain.Common.Http;
using eShop.Domain.Requests.Auth;

namespace eAccount.Blazor.Server.Domain.Interfaces;

public interface IUserService
{
    public ValueTask<HttpResponse> GetUserAsync(Guid id);
    public ValueTask<HttpResponse> GetUserPrimaryEmailAsync(Guid id);
    public ValueTask<HttpResponse> GetUserPrimaryPhoneNumberAsync(Guid id);
    public ValueTask<HttpResponse> GetUserEmailsAsync(Guid id);
    public ValueTask<HttpResponse> GetUserDevicesAsync(Guid id);
    public ValueTask<HttpResponse> GetUserLoginMethodsAsync(Guid id);
    public ValueTask<HttpResponse> GetUserVerificationDataAsync(Guid id);
    public ValueTask<HttpResponse> GetUserLinkedAccountsDataAsync(Guid id);
    public ValueTask<HttpResponse> GetUserPhoneNumbersAsync(Guid id);
    public ValueTask<HttpResponse> GetUserStateAsync(Guid id);
    public ValueTask<HttpResponse> GetUserPersonalDataAsync(Guid id);
    public ValueTask<HttpResponse> GetTwoFactorMethodsAsync(Guid id);
    public ValueTask<HttpResponse> GetLockoutStateAsync(Guid id);
    public ValueTask<HttpResponse> ChangeUsernameAsync(ChangeUsernameRequest request);
    public ValueTask<HttpResponse> AddPersonalDataAsync(AddPersonalDataRequest request);
    public ValueTask<HttpResponse> ChangePersonalDataAsync(ChangePersonalDataRequest request);
    public ValueTask<HttpResponse> RemovePersonalDataAsync(RemovePersonalDataRequest request);
}