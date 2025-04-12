using eShop.Domain.Abstraction.Services;
using eShop.Domain.Common.API;

namespace eShop.Infrastructure.Services;

public class BrandService(
    IHttpClientService clientService,
    IConfiguration configuration) : IBrandService, IApi
{
    private readonly IHttpClientService clientService = clientService;
    private readonly IConfiguration configuration = configuration;
    public async ValueTask<Response> GetBrandsListAsync() => await clientService.SendAsync(
        new Request(Url: $"{configuration["Configuration:Services:Proxy:Gateway:Uri"]}/api/v1/Brands", Methods: HttpMethods.Get));
}