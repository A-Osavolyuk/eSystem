using eAccount.Blazor.Server.Domain.Abstraction.Services;

namespace eAccount.Blazor.Server.Infrastructure.Implementations;

public class TypeService(
    IConfiguration configuration, 
    IApiClient apiClient) : ApiService(configuration, apiClient), ITypeService
{
    private const string BasePath = "api/v1/Types";
    
    public async ValueTask<HttpResponse> GetAllAsync() =>
        await ApiClient.SendAsync(
            new HttpRequest { Url = $"{Gateway}/{BasePath}/", Method = HttpMethod.Get }, 
            new HttpOptions { Type = DataType.Text });
}