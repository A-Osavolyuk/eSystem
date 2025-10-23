using eAccount.Blazor.Server.Domain.Abstraction.Services;
using eAccount.Blazor.Server.Domain.Interfaces;
using eShop.Domain.Common.Http;
using eShop.Domain.Enums;

namespace eAccount.Blazor.Server.Infrastructure.Implementations;

public class UnitService(
    IConfiguration configuration, 
    IApiClient apiClient) : ApiService(configuration, apiClient), IUnitService
{
    private const string BasePath = "api/v1/Category";
    
    public async ValueTask<HttpResponse> GetAllAsync() =>
        await ApiClient.SendAsync(
            new HttpRequest { Url = $"{Gateway}/{BasePath}", Method = HttpMethod.Get }, 
            new HttpOptions { WithBearer = false, Type = DataType.Text });
}