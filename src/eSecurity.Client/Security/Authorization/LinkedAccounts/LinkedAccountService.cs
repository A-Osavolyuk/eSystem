using eSecurity.Client.Common.Http;
using eSecurity.Core.Common.Requests;

namespace eSecurity.Client.Security.Authorization.LinkedAccounts;

public class LinkedAccountService(IApiClient apiClient) : ILinkedAccountService
{
    private readonly IApiClient _apiClient = apiClient;

    public async ValueTask<ApiResponse> DisconnectAsync(DisconnectLinkedAccountRequest request)
        => await _apiClient.SendAsync(
            new ApiRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/LinkedAccount/disconnect"
            }, new ApiOptions()
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.Bearer
            });
}