using eShop.Domain.Abstraction.Services;
using eShop.Domain.Common.API;
using eShop.Domain.Enums;
using eShop.Domain.Options;

namespace eShop.Infrastructure.Services;

public class CurrencyService(
    IConfiguration configuration, 
    IApiClient apiClient) : ApiService(configuration, apiClient), ICurrencyService
{
    public async ValueTask<Response> GetAllAsync() =>
        await ApiClient.SendAsync(
            new HttpRequest { Url = $"{Gateway}/api/v1/Currency/", Method = HttpMethod.Get }, 
            new HttpOptions { WithBearer = false, Type = DataType.Text });
}