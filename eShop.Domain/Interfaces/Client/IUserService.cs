using eShop.Domain.Common.API;
using eShop.Domain.Common.Http;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Domain.Interfaces.Client;

public interface IUserService
{
    public ValueTask<HttpResponse> GetUserAsync(Guid id);
    public ValueTask<HttpResponse> GetUserStateAsync(Guid id);
    public ValueTask<HttpResponse> GetUserSecurityDataAsync(Guid id);
    public ValueTask<HttpResponse> GetUserPersonalDataAsync(Guid id);
    public ValueTask<HttpResponse> GetTwoFactorProvidersAsync(Guid id);
    public ValueTask<HttpResponse> GetLockoutStateAsync(Guid id);
    public ValueTask<HttpResponse> ChangeUsernameAsync(ChangeUsernameRequest request);
    public ValueTask<HttpResponse> AddPersonalDataAsync(AddPersonalDataRequest request);
    public ValueTask<HttpResponse> ChangePersonalDataAsync(ChangePersonalDataRequest request);
    public ValueTask<HttpResponse> RemovePersonalDataAsync(RemovePersonalDataRequest request);
}