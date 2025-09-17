using eShop.Domain.Common.Http;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Blazor.Server.Domain.Interfaces;

public interface IOAuthService
{
    public ValueTask<HttpResponse> LoadSessionAsync(LoadOAuthSessionRequest request);
    public ValueTask<HttpResponse> DisconnectAsync(DisconnectLinkedAccountRequest request);
    public ValueTask<HttpResponse> AllowAsync(AllowLinkedAccountRequest request);
    public ValueTask<HttpResponse> DisallowAsync(DisallowLinkedAccountRequest request);
}