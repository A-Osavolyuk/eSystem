using eSystem.Core.Requests.Auth;

namespace eAccount.Security.Authentication.ODIC;

public interface IConnectService
{
    public ValueTask<HttpResponse> TokenAsync(TokenRequest request);
    public ValueTask<HttpResponse> AuthorizeAsync(AuthorizeRequest request);
    public ValueTask<HttpResponse> LogoutAsync(LogoutRequest request);
}