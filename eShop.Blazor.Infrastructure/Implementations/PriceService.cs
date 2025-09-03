using eShop.Blazor.Domain.Abstraction.Services;
using eShop.Blazor.Domain.Interfaces;
using eShop.Domain.Common.Http;
using eShop.Domain.Enums;

namespace eShop.Blazor.Infrastructure.Implementations;

public class PriceService(
    IConfiguration configuration, 
    IApiClient apiClient) : ApiService(configuration, apiClient), IPriceService
{
    public async ValueTask<HttpResponse> GetAllAsync() =>
        await ApiClient.SendAsync(
            new HttpRequest { Url = $"{Gateway}/api/v1/Price/", Method = HttpMethod.Get }, 
            new HttpOptions { WithBearer = false, Type = DataType.Text });
}