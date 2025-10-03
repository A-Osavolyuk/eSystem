using eShop.Blazor.Server.Domain.Abstraction.Services;
using eShop.Blazor.Server.Domain.Interfaces;
using eShop.Domain.Common.Http;
using eShop.Domain.Enums;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Blazor.Server.Infrastructure.Implementations;

public class VerificationService(
    IConfiguration configuration, 
    IApiClient apiClient) : ApiService(configuration, apiClient), IVerificationService
{
    public async ValueTask<HttpResponse> SendCodeAsync(SendCodeRequest request) => await ApiClient.SendAsync(
        new HttpRequest { Url = $"{Gateway}/api/v1/Verification/code/send", Method = HttpMethod.Post, Data = request },
        new HttpOptions { WithBearer = false, Type = DataType.Text });

    public async ValueTask<HttpResponse> ResendCodeAsync(ResendCodeRequest request) => await ApiClient.SendAsync(
        new HttpRequest { Url = $"{Gateway}/api/v1/Verification/code/resend", Method = HttpMethod.Post, Data = request },
        new HttpOptions { WithBearer = false, Type = DataType.Text });

    public async ValueTask<HttpResponse> VerifyCodeAsync(VerifyCodeRequest request) => await ApiClient.SendAsync(
        new HttpRequest { Url = $"{Gateway}/api/v1/Verification/code/verify", Method = HttpMethod.Post, Data = request },
        new HttpOptions { WithBearer = false, Type = DataType.Text });
}