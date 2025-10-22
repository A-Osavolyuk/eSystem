using eShop.Domain.Common.Http;
using eShop.Domain.Requests.Auth;

namespace eShop.Blazor.Server.Domain.Interfaces;

public interface IOAuthService
{
    public ValueTask<HttpResponse> LoadSessionAsync(LoadOAuthSessionRequest request);
    public ValueTask<HttpResponse> DisconnectAsync(DisconnectLinkedAccountRequest request);
}