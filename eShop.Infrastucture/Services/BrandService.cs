namespace eShop.Infrastructure.Services;

public class BrandSevice(
    IHttpClientService clientService,
    IConfiguration configuration) : IBrandService
{
    private readonly IHttpClientService clientService = clientService;
    private readonly IConfiguration configuration = configuration;
    public async ValueTask<Response> GetBrandsListAsync() => await clientService.SendAsync(
        new Request(Url: $"{configuration["Configuration:Services:Proxy:Gateway:Uri"]}/api/v1/Brands", Methods: HttpMethods.Get));
}