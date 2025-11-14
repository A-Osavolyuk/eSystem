using eSecurity.Core.Common.Requests;

namespace eSecurity.Client.Services.Interfaces;

public interface IConnectService
{
    public ValueTask<HttpResponse> GetPublicKeyAsync();
    public ValueTask<HttpResponse> AuthorizeAsync(AuthorizeRequest request);
    public ValueTask<HttpResponse> TokenAsync(TokenRequest request);
    public ValueTask<HttpResponse> LogoutAsync(LogoutRequest request);
}