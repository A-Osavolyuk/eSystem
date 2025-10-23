using eSystem.Domain.Common.Http;
using eSystem.Domain.Requests.Auth;

namespace eAccount.Blazor.Server.Domain.Interfaces;

public interface IOAuthService
{
    public ValueTask<HttpResponse> LoadSessionAsync(LoadOAuthSessionRequest request);
    public ValueTask<HttpResponse> DisconnectAsync(DisconnectLinkedAccountRequest request);
}