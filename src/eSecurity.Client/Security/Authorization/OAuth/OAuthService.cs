using eSecurity.Client.Common.Http;

namespace eSecurity.Client.Security.Authorization.OAuth;

public class OAuthService(IApiClient apiClient) : IOAuthService
{
    private readonly IApiClient _apiClient = apiClient;
}