using eShop.Domain.Common.API;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Domain.Interfaces.Client;

public interface IUserService
{
    public ValueTask<Response> GetUserAsync(Guid id);
    public ValueTask<Response> GetUserSecurityDataAsync(Guid id);
    public ValueTask<Response> GetUserPersonalDataAsync(Guid id);
    public ValueTask<Response> GetTwoFactorProvidersAsync(Guid id);
    public ValueTask<Response> GetLockoutStateAsync(Guid id);
    public ValueTask<Response> ChangeUsernameAsync(ChangeUsernameRequest request);
    public ValueTask<Response> AddPersonalDataAsync(AddPersonalDataRequest request);
    public ValueTask<Response> ChangePersonalDataAsync(ChangePersonalDataRequest request);
    public ValueTask<Response> RemovePersonalDataAsync(RemovePersonalDataRequest request);
}