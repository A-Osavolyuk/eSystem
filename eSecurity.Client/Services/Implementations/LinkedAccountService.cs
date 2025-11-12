using eSecurity.Client.Common.Http;
using eSecurity.Client.Services.Interfaces;
using eSecurity.Core.Common.Requests;
using eSystem.Core.Common.Http;

namespace eSecurity.Client.Services.Implementations;

public class LinkedAccountService(IApiClient apiClient) : ILinkedAccountService
{
    private readonly IApiClient _apiClient = apiClient;

    public async ValueTask<HttpResponse> DisconnectAsync(DisconnectLinkedAccountRequest request)
        => await _apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/LinkedAccount/disconnect"
            }, new HttpOptions() { Type = DataType.Text, WithBearer = true });
}