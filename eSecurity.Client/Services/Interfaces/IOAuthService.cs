using eSecurity.Core.Common.Requests;

namespace eSecurity.Client.Services.Interfaces;

public interface IOAuthService
{
    public ValueTask<HttpResponse> LoadSessionAsync(LoadOAuthSessionRequest request);
}