using eShop.Domain.Abstraction.Services;
using eShop.Domain.Common.API;
using eShop.Domain.Options;

namespace eShop.Infrastructure.Services;

public class BrandService(
    IConfiguration configuration,
    IApiClient apiClient) : ApiService(configuration, apiClient), IBrandService
{
    public async ValueTask<Response> GetBrandsListAsync() => await ApiClient.SendAsync(
        new HttpRequest(Url: $"{Configuration[Key]}/api/v1/Brands", Method: HttpMethod.Get),
        new HttpOptions() { WithBearer = true, ValidateToken = true });
}