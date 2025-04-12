using eShop.Domain.Abstraction.Services;
using eShop.Domain.Common.API;
using eShop.Domain.Requests.API.Seller;

namespace eShop.Infrastructure.Services;

public class SellerService(
    IHttpClientService httpClient, 
    IConfiguration configuration) : ApiService(configuration, httpClient), ISellerService
{
    public async ValueTask<Response> RegisterSellerAsync(RegisterSellerRequest request) =>
        await HttpClientService.SendAsync(new Request(
            Url: $"{Configuration[Key]}/api/v1/Seller/register-seller",
            Methods: HttpMethods.Post,
            Data: request));
}