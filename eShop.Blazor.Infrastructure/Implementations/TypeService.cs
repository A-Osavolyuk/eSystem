using eShop.Blazor.Domain.Abstraction.Services;
using eShop.Blazor.Domain.Interfaces;
using eShop.Domain.Common.Http;
using eShop.Domain.Enums;

namespace eShop.Blazor.Infrastructure.Implementations;

public class TypeService(
    IConfiguration configuration, 
    IApiClient apiClient) : ApiService(configuration, apiClient), ITypeService
{
    public async ValueTask<HttpResponse> GetAllAsync() =>
        await ApiClient.SendAsync(
            new HttpRequest { Url = $"{Gateway}/api/v1/Types/", Method = HttpMethod.Get }, 
            new HttpOptions { WithBearer = false, Type = DataType.Text });
}