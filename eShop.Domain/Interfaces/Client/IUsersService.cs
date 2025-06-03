using eShop.Domain.Common.API;

namespace eShop.Domain.Interfaces.Client;

public interface IUsersService
{
    public ValueTask<Response> GetTwoFactorProvidersAsync(Guid id);
    public ValueTask<Response> GetTwoFactorStateAsync(Guid id);
    public ValueTask<Response> GetLockoutStateAsync(Guid id);
}