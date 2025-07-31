using eShop.Domain.Abstraction.Services;
using eShop.Domain.Common.API;
using eShop.Domain.Enums;
using eShop.Domain.Options;

namespace eShop.Infrastructure.Services;

public class CategoryService(
    IConfiguration configuration, 
    IApiClient apiClient) : ApiService(configuration, apiClient), ICategoryService
{
    public async ValueTask<Response> GetAllAsync() =>
        await ApiClient.SendAsync(
            new HttpRequest { Url = $"{Gateway}/api/v1/Category/", Method = HttpMethod.Get }, 
            new HttpOptions { WithBearer = false, Type = DataType.Text });
}