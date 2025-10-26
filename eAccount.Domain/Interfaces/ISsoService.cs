using eSystem.Core.Common.Http;
using eSystem.Core.Requests.Auth;

namespace eAccount.Domain.Interfaces;

public interface ISsoService
{
    public ValueTask<HttpResponse> RefreshTokenAsync(RefreshTokenRequest request);
    public ValueTask<HttpResponse> GenerateTokenAsync(GenerateTokenRequest request);
    public ValueTask<HttpResponse> AuthorizeAsync(AuthorizeRequest request);
    public ValueTask<HttpResponse> UnauthorizeAsync(UnauthorizeRequest request);
}