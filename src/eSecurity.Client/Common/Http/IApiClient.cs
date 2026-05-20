namespace eSecurity.Client.Common.Http;

public interface IApiClient
{
    public ValueTask<ApiResponse> SendAsync(ApiRequest request, CancellationToken cancellationToken = default);
}