using eShop.Domain.Common.API;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Domain.Interfaces.Client;

public interface IOAuthService
{
    public ValueTask<Response> LoadSessionAsync(LoadOAuthSessionRequest request);
    public ValueTask<Response> DisconnectAsync(DisconnectLinkedAccountRequest request);
    public ValueTask<Response> AllowAsync(AllowLinkedAccountRequest request);
    public ValueTask<Response> DisallowAsync(DisallowLinkedAccountRequest request);
    public ValueTask<Response> ConfirmDisconnectAsync(ConfirmDisconnectLinkedAccountRequest request);
    public ValueTask<Response> ConfirmAllowAsync(ConfirmAllowLinkedAccountRequest request);
    public ValueTask<Response> ConfirmDisallowAsync(ConfirmDisallowLinkedAccountRequest request);
}