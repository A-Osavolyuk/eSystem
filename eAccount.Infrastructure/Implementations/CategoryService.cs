using eAccount.Domain.Abstraction.Services;

namespace eAccount.Infrastructure.Implementations;

public class CategoryService(
    IConfiguration configuration, 
    IApiClient apiClient) : ApiService(configuration, apiClient), ICategoryService
{
    private const string BasePath = "api/v1/Category";
    
    public async ValueTask<HttpResponse> GetAllAsync() =>
        await ApiClient.SendAsync(
            new HttpRequest { Url = $"{Gateway}/{BasePath}", Method = HttpMethod.Get }, 
            new HttpOptions { Type = DataType.Text });
}