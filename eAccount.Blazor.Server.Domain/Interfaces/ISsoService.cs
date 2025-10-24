using eSystem.Domain.Common.Http;
using eSystem.Domain.Requests.Auth;

namespace eAccount.Blazor.Server.Domain.Interfaces;

public interface ISsoService
{
    public ValueTask<HttpResponse> RefreshTokenAsync(RefreshTokenRequest request);
    public ValueTask<HttpResponse> GenerateTokenAsync(GenerateTokenRequest request);
    public ValueTask<HttpResponse> AuthorizeAsync(AuthorizeRequest request);
    public ValueTask<HttpResponse> UnauthorizeAsync(UnauthorizeRequest request);
}