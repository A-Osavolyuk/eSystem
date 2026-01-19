namespace eSecurity.Client.Common.Http;

public interface IApiClient
{
    public ValueTask<ApiResponse> SendAsync(
        ApiRequest request, 
        ApiOptions options, 
        CancellationToken cancellationToken = default);
}