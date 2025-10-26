using eAccount.Domain.Abstraction.Services;
using eSystem.Core.Requests.Auth;

namespace eAccount.Infrastructure.Implementations;

public class SsoService(    
    IConfiguration configuration, 
    IApiClient apiClient) : ApiService(configuration, apiClient), ISsoService
{
    private const string BasePath = "api/v1/Sso";
    
    public async ValueTask<HttpResponse> RefreshTokenAsync(RefreshTokenRequest request) => await ApiClient.SendAsync(
        new HttpRequest { Url = $"{Gateway}/{BasePath}/token", Method = HttpMethod.Post, Data = request },
        new HttpOptions { Type = DataType.Text });

    public async ValueTask<HttpResponse> GenerateTokenAsync(GenerateTokenRequest request) => await ApiClient.SendAsync(
        new HttpRequest { Url = $"{Gateway}/{BasePath}/refresh", Method = HttpMethod.Post, Data = request },
        new HttpOptions { Type = DataType.Text });

    public async ValueTask<HttpResponse> AuthorizeAsync(AuthorizeRequest request) => await ApiClient.SendAsync(
        new HttpRequest { Url = $"{Gateway}/{BasePath}/authorize", Method = HttpMethod.Post, Data = request },
        new HttpOptions { Type = DataType.Text });


    public async ValueTask<HttpResponse> UnauthorizeAsync(UnauthorizeRequest request) => await ApiClient.SendAsync(
        new HttpRequest { Url = $"{Gateway}/{BasePath}/unauthorize", Method = HttpMethod.Post, Data = request },
        new HttpOptions { Type = DataType.Text });
}