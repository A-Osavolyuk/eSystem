using eShop.Domain.Common.API;
using eShop.Domain.Requests.API.Seller;

namespace eShop.Domain.Interfaces.Client;

public interface ISellerService
{
    public ValueTask<Response> RegisterSellerAsync(RegisterSellerRequest request);
}