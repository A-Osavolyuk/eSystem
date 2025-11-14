using eSecurity.Core.Common.Requests;

namespace eSecurity.Client.Security.Authorization.OAuth;

public interface IOAuthService
{
    public ValueTask<HttpResponse> LoadSessionAsync(LoadOAuthSessionRequest request);
}