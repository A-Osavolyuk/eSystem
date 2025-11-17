using eSecurity.Core.Common.Requests;

namespace eSecurity.Client.Security.Authentication.Oidc;

public interface IConnectService
{
    public ValueTask<Result> GetPublicKeysAsync();
    public ValueTask<Result> GetOpenidConfigurationAsync();
    public ValueTask<Result> AuthorizeAsync(AuthorizeRequest request);
    public ValueTask<Result> TokenAsync(TokenRequest request);
    public ValueTask<Result> LogoutAsync(LogoutRequest request);
}