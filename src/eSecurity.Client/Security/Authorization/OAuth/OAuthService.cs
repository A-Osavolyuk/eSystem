namespace eSecurity.Client.Security.Authorization.OAuth;

public class OAuthService(IApiClient apiClient) : IOAuthService
{
    private readonly IApiClient _apiClient = apiClient;
    
    public async ValueTask<ApiResponse> GetSessionAsync(Guid id)
    {
        return await _apiClient.SendAsync(new ApiRequest()
        {
            Method = HttpMethod.Get,
            Url = $"/api/v1/OAuth/{id}"
        }, new ApiOptions()
        {
            Authentication = AuthenticationType.None,
            ContentType = ContentTypes.Application.Json
        });
    }
}