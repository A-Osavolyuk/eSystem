using eSystem.Core.Common.Http;
using eSystem.Core.Requests.Auth;

namespace eAccount.Domain.Interfaces;

public interface ISsoService
{
    public ValueTask<HttpResponse> TokenAsync(TokenRequest request);
    public ValueTask<HttpResponse> AuthorizeAsync(AuthorizeRequest request);
    public ValueTask<HttpResponse> UnauthorizeAsync(SignOutRequest request);
}