using eShop.Domain.Abstraction.Services;
using eShop.Domain.Common.API;
using eShop.Domain.Enums;
using eShop.Domain.Options;

namespace eShop.Infrastructure.Services;

public class PriceService(
    IConfiguration configuration, 
    IApiClient apiClient) : ApiService(configuration, apiClient), IPriceService
{
    public async ValueTask<Response> GetAllAsync() =>
        await ApiClient.SendAsync(
            new HttpRequest { Url = $"{Gateway}/api/v1/Price/", Method = HttpMethod.Get }, 
            new HttpOptions { ValidateToken = false, WithBearer = false, Type = DataType.Text });
}