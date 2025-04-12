using eShop.Domain.Common.API;
using eShop.Domain.Requests.API.Cart;

namespace eShop.Domain.Interfaces.Client;

public interface ICartService
{
    public ValueTask<Response> GetCartAsync(Guid userId);
    public ValueTask<Response> UpdateCartAsync(UpdateCartRequest request);
}