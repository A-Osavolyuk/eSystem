using eShop.Domain.Common.API;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Domain.Interfaces.Client;

public interface IUsersService
{
    public ValueTask<Response> GetUserAsync(Guid id);
    public ValueTask<Response> GetTwoFactorProvidersAsync(Guid id);
    public ValueTask<Response> GetTwoFactorStateAsync(Guid id);
    public ValueTask<Response> GetLockoutStateAsync(Guid id);
    public ValueTask<Response> GetPersonalDataAsync(Guid id);
    public ValueTask<Response> ChangeUsernameAsync(ChangeUserNameRequest request);
    public ValueTask<Response> ChangePersonalDataAsync(ChangePersonalDataRequest request);
}