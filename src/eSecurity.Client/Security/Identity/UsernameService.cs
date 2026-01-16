using eSecurity.Client.Common.Http;
using eSecurity.Core.Common.Requests;

namespace eSecurity.Client.Security.Identity;

public class UsernameService(IApiClient apiClient) : IUsernameService
{
    private readonly IApiClient _apiClient = apiClient;

    public async ValueTask<HttpResponse> SetUsernameAsync(SetUsernameRequest request)
        => await _apiClient.SendAsync(
            new HttpRequest()
            {
                Data = request,
                Method = HttpMethod.Post,
                Url = "api/v1/Username/set"
            }, new ApiOptions()
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.None
            });

    public async ValueTask<HttpResponse> ChangeUsernameAsync(ChangeUsernameRequest request)
        => await _apiClient.SendAsync(
            new HttpRequest()
            {
                Data = request,
                Method = HttpMethod.Put,
                Url = "api/v1/Username/change"
            }, new ApiOptions()
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.Bearer
            });

    public async ValueTask<HttpResponse> CheckUsernameAsync(CheckUsernameRequest request)
        => await _apiClient.SendAsync(
            new HttpRequest()
            {
                Data = request,
                Method = HttpMethod.Post,
                Url = "api/v1/Username/check"
            }, new ApiOptions()
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.None
            });
}