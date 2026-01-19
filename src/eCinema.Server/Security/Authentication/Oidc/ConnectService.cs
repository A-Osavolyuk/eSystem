using eSystem.Core.Http;
using eSystem.Core.Http.Constants;
using eSystem.Core.Security.Authentication.Oidc.Token;

namespace eCinema.Server.Security.Authentication.Oidc;

public class ConnectService(
    IApiClient apiClient,
    IOpenIdDiscoveryProvider openIdDiscoveryProvider) : IConnectService
{
    private readonly IApiClient _apiClient = apiClient;
    private readonly IOpenIdDiscoveryProvider _openIdDiscoveryProvider = openIdDiscoveryProvider;

    public async ValueTask<ApiResponse> TokenAsync(TokenRequest request, 
        CancellationToken cancellationToken = default)
    {
        var openIdConfiguration = await _openIdDiscoveryProvider.GetOpenIdConfigurationsAsync(cancellationToken);
        if (openIdConfiguration is null)
        {
            return ApiResponse.Fail(new Error()
            {
                Code = ErrorTypes.OAuth.ServerError,
                Description = "Server error"
            });
        }

        var apiRequest = new ApiRequest()
        {
            Method = HttpMethod.Post,
            Url = openIdConfiguration.TokenEndpoint,
            Data = request
        };

        var apiOptions = new ApiOptions()
        {
            Authentication = AuthenticationType.Basic,
            ContentType = ContentTypes.Application.XwwwFormUrlEncoded
        };

        return await _apiClient.SendAsync(apiRequest, apiOptions, cancellationToken);
    }
}