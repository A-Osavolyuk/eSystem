using eShop.Blazor.Domain.Abstraction.Services;
using eShop.Blazor.Domain.Interfaces;
using eShop.Domain.Common.Http;
using eShop.Domain.Enums;

namespace eShop.Blazor.Infrastructure.Implementations;

public class CurrencyService(
    IConfiguration configuration, 
    IApiClient apiClient) : ApiService(configuration, apiClient), ICurrencyService
{
    public async ValueTask<HttpResponse> GetAllAsync() =>
        await ApiClient.SendAsync(
            new HttpRequest { Url = $"{Gateway}/api/v1/Currency/", Method = HttpMethod.Get }, 
            new HttpOptions { WithBearer = false, Type = DataType.Text });
}