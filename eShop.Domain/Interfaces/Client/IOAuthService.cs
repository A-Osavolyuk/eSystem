using eShop.Domain.Common.API;
using eShop.Domain.Common.Http;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Domain.Interfaces.Client;

public interface IOAuthService
{
    public ValueTask<HttpResponse> LoadSessionAsync(LoadOAuthSessionRequest request);
    public ValueTask<HttpResponse> DisconnectAsync(DisconnectLinkedAccountRequest request);
    public ValueTask<HttpResponse> AllowAsync(AllowLinkedAccountRequest request);
    public ValueTask<HttpResponse> DisallowAsync(DisallowLinkedAccountRequest request);
}