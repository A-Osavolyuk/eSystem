using eSecurity.Client.Common.Http;
using eSecurity.Core.Common.Requests;
using eSecurity.Core.Common.Responses;

namespace eSecurity.Client.Security.Authorization.OAuth;

public class OAuthService(IApiClient apiClient) : IOAuthService
{
    private readonly IApiClient _apiClient = apiClient;
}