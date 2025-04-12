using eShop.Domain.Abstraction.Services;
using eShop.Domain.Common.API;

namespace eShop.Infrastructure.Services;

public class BrandService(
    IConfiguration configuration, 
    IHttpClientService httpClientService) : Api(configuration, httpClientService), IBrandService
{
    public async ValueTask<Response> GetBrandsListAsync() => await HttpClientService.SendAsync(
        new Request(Url: $"{Configuration[Key]}/api/v1/Brands", Methods: HttpMethods.Get));
}