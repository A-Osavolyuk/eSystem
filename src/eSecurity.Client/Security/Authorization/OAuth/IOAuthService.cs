namespace eSecurity.Client.Security.Authorization.OAuth;

public interface IOAuthService
{
    public ValueTask<ApiResponse> GetSessionAsync(Guid id);
}