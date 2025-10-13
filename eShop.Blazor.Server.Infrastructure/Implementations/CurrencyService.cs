using eShop.Blazor.Server.Domain.Abstraction.Services;
using eShop.Blazor.Server.Domain.Interfaces;
using eShop.Domain.Common.Http;
using eShop.Domain.Enums;

namespace eShop.Blazor.Server.Infrastructure.Implementations;

public class CurrencyService(
    IConfiguration configuration, 
    IApiClient apiClient) : ApiService(configuration, apiClient), ICurrencyService
{
    private const string BasePath = "api/v1/Currency";
    public async ValueTask<HttpResponse> GetAllAsync() =>
        await ApiClient.SendAsync(
            new HttpRequest { Url = $"{Gateway}/{BasePath}", Method = HttpMethod.Get }, 
            new HttpOptions { WithBearer = false, Type = DataType.Text });
}