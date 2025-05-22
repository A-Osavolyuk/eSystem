using eShop.Domain.Abstraction.Services;
using eShop.Domain.Common.API;
using eShop.Domain.Options;
using eShop.Domain.Requests.API.Seller;

namespace eShop.Infrastructure.Services;

public class SellerService(
    IApiClient httpClient, 
    IConfiguration configuration) : ApiService(configuration, httpClient), ISellerService
{
    public async ValueTask<Response> RegisterSellerAsync(RegisterSellerRequest request) =>
        await ApiClient.SendAsync(new HttpRequest(
            Url: $"{Configuration[Key]}/api/v1/Seller/register-seller",
            Method: HttpMethod.Post,
            Data: request),         
            new HttpOptions() { ValidateToken = true, WithBearer = true });
}