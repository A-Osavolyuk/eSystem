using eSecurity.Core.Common.Requests;
using eSecurity.Core.Common.Responses;
using eSystem.Core.Common.Http;

namespace eSecurity.Client.Security.Authorization.OAuth;

public interface IOAuthService
{
    public ValueTask<HttpResponse<LoadOAuthSessionResponse>> LoadSessionAsync(LoadOAuthSessionRequest request);
}