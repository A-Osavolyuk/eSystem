namespace eSecurity.Client.Common.Http;

public interface IApiClient
{
    public ValueTask<ApiResponse> SendAsync(
        ApiRequest apiRequest, 
        ApiOptions apiOptions, 
        CancellationToken cancellationToken = default);
}