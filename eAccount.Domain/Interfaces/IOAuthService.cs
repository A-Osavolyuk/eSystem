using eSystem.Core.Common.Http;
using eSystem.Core.Requests.Auth;

namespace eAccount.Domain.Interfaces;

public interface IOAuthService
{
    public ValueTask<HttpResponse> LoadSessionAsync(LoadOAuthSessionRequest request);
    public ValueTask<HttpResponse> DisconnectAsync(DisconnectLinkedAccountRequest request);
}