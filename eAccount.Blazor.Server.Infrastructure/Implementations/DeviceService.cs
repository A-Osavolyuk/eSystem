using eAccount.Blazor.Server.Domain.Abstraction.Services;

namespace eAccount.Blazor.Server.Infrastructure.Implementations;

public class DeviceService(
    IConfiguration configuration, 
    IApiClient apiClient) : ApiService(configuration, apiClient), IDeviceService
{
    private const string BasePath = "api/v1/Device";
    
    public async ValueTask<HttpResponse> GetAsync(Guid id) => await ApiClient.SendAsync(
        new HttpRequest { Url = $"{Gateway}/{BasePath}/{id}", Method = HttpMethod.Get, },
        new HttpOptions { Type = DataType.Text });

    public async ValueTask<HttpResponse> TrustAsync(TrustDeviceRequest request) => await ApiClient.SendAsync(
        new HttpRequest { Url = $"{Gateway}/{BasePath}/trust", Method = HttpMethod.Post, Data = request },
        new HttpOptions { Type = DataType.Text });

    public async ValueTask<HttpResponse> BlockAsync(BlockDeviceRequest request) => await ApiClient.SendAsync(
        new HttpRequest { Url = $"{Gateway}/{BasePath}/block", Method = HttpMethod.Post, Data = request },
        new HttpOptions { Type = DataType.Text });

    public async ValueTask<HttpResponse> UnblockAsync(UnblockDeviceRequest request) => await ApiClient.SendAsync(
        new HttpRequest { Url = $"{Gateway}/{BasePath}/unblock", Method = HttpMethod.Post, Data = request },
        new HttpOptions { Type = DataType.Text });

    public async ValueTask<HttpResponse> VerifyAsync(VerifyDeviceRequest request) => await ApiClient.SendAsync(
        new HttpRequest { Url = $"{Gateway}/{BasePath}/verify", Method = HttpMethod.Post, Data = request },
        new HttpOptions { Type = DataType.Text });
}