namespace eSecurity.Client.Common.Http;

public interface IApiClient
{
    public ValueTask<ApiResponse<TResponse>> SendAsync<TResponse>(
        ApiRequest apiRequest, 
        ApiOptions apiOptions, 
        CancellationToken cancellationToken = default);
    
    public ValueTask<HttpResponse> SendAsync(
        ApiRequest apiRequest, 
        ApiOptions apiOptions, 
        CancellationToken cancellationToken = default);
}